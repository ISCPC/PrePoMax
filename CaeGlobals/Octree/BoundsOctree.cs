﻿// <copyright file="BoundsOctree.cs">
//     Distributed under the BSD Licence (see LICENCE file).
//     
//     Copyright (c) 2014, Nition, http://www.momentstudio.co.nz/
//     Copyright (c) 2017, Máté Cserép, http://codenet.hu
//     All rights reserved.
// </copyright>
namespace Octree
{
    using System.Collections.Generic;

    /// <summary>
    /// A Dynamic, Loose Octree for storing any objects that can be described with AABB bounds
    /// </summary>
    /// <seealso cref="BoundsOctree{T}"/>
    /// <remarks>
    /// Octree:	An octree is a tree data structure which divides 3D space into smaller partitions (nodes)
    /// and places objects into the appropriate nodes. This allows fast access to objects
    /// in an area of interest without having to check every object.
    /// 
    /// Dynamic: The octree grows or shrinks as required when objects as added or removed.
    /// It also splits and merges nodes as appropriate. There is no maximum depth.
    /// Nodes have a constant - <see cref="BoundsOctree{T}.Node.NumObjectsAllowed"/> - which sets the amount of items allowed in a node before it splits.
    /// 
    /// Loose: The octree's nodes can be larger than 1/2 their parent's length and width, so they overlap to some extent.
    /// This can alleviate the problem of even tiny objects ending up in large nodes if they're near boundaries.
    /// A looseness value of 1.0 will make it a "normal" octree.
    /// 
    /// Note: For loops are often used here since in some cases (e.g. the IsColliding method)
    /// they actually give much better performance than using Foreach, even in the compiled build.
    /// Using a LINQ expression is worse again than Foreach.
    /// 
    /// See also: PointOctree, where objects are stored as single points and some code can be simplified
    /// </remarks>
    /// <typeparam name="T">The content of the octree can be anything, since the bounds data is supplied separately.</typeparam>
    public partial class BoundsOctree<T>
    {
        /// <summary>
        /// Root node of the octree
        /// </summary>
        private Node _rootNode;

        /// <summary>
        /// Should be a value between 1 and 2. A multiplier for the base size of a node.
        /// </summary>
        /// <remarks>
        /// 1.0 is a "normal" octree, while values > 1 have overlap
        /// </remarks>
        private readonly double _looseness;

        /// <summary>
        /// Size that the octree was on creation
        /// </summary>
        private readonly double _initialSize;

        /// <summary>
        /// Minimum side length that a node can be - essentially an alternative to having a max depth
        /// </summary>
        private readonly double _minSize;

	    /// <summary>
	    /// The total amount of objects currently in the tree
	    /// </summary>
	    public int Count { get; private set; }

		/// <summary>
		/// Gets the bounding box that represents the whole octree
		/// </summary>
		/// <value>The bounding box of the root node.</value>
		public BoundingBox MaxBounds
        {
            get { return _rootNode.Bounds; }
        }

        /// <summary>
        /// Constructor for the bounds octree.
        /// </summary>
        /// <param name="initialWorldSize">Size of the sides of the initial node, in metres. The octree will never shrink smaller than this.</param>
        /// <param name="initialWorldPos">Position of the center of the initial node.</param>
        /// <param name="minNodeSize">Nodes will stop splitting if the new nodes would be smaller than this (metres).</param>
        /// <param name="loosenessVal">Clamped between 1 and 2. Values > 1 let nodes overlap.</param>
        public BoundsOctree(double initialWorldSize, Point initialWorldPos, double minNodeSize, double loosenessVal)
        {
            if (minNodeSize > initialWorldSize)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Minimum node size must be at least as big as the initial world size. Was: " + minNodeSize
                    + " Adjusted to: " + initialWorldSize);
                minNodeSize = initialWorldSize;
            }
            Count = 0;
            _initialSize = initialWorldSize;
            _minSize = minNodeSize;
            _looseness = MathExtensions.Clamp(loosenessVal, 1.0f, 2.0f);
            _rootNode = new Node(_initialSize, _minSize, _looseness, initialWorldPos);
        }

        // #### PUBLIC METHODS ####

        /// <summary>
        /// Add an object.
        /// </summary>
        /// <param name="obj">Object to add.</param>
        /// <param name="objBounds">3D bounding box around the object.</param>
        public void Add(T obj, BoundingBox objBounds)
        {
            // Add object or expand the octree until it can be added
            int count = 0; // Safety check against infinite/excessive growth
            while (!_rootNode.Add(obj, objBounds))
            {
                Grow(objBounds.Center - _rootNode.Center);
                if (++count > 20)
                {
                    throw new System.Exception(
                        "Aborted Add operation as it seemed to be going on forever (" + (count - 1)
                        + ") attempts at growing the octree.");
                }
            }
            Count++;
        }

        /// <summary>
        /// Remove an object. Makes the assumption that the object only exists once in the tree.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <returns>True if the object was removed successfully.</returns>
        public bool Remove(T obj)
        {
            bool removed = _rootNode.Remove(obj);

            // See if we can shrink the octree down now that we've removed the item
            if (removed)
            {
                Count--;
                Shrink();
            }

            return removed;
        }

        /// <summary>
        /// Removes the specified object at the given position. Makes the assumption that the object only exists once in the tree.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <param name="objBounds">3D bounding box around the object.</param>
        /// <returns>True if the object was removed successfully.</returns>
        public bool Remove(T obj, BoundingBox objBounds)
        {
            bool removed = _rootNode.Remove(obj, objBounds);

            // See if we can shrink the octree down now that we've removed the item
            if (removed)
            {
                Count--;
                Shrink();
            }

            return removed;
        }

        /// <summary>
        /// Check if the specified bounds intersect with anything in the tree. See also: GetColliding.
        /// </summary>
        /// <param name="checkBounds">bounds to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool IsColliding(BoundingBox checkBounds)
        {
            return _rootNode.IsColliding(ref checkBounds);
        }

        /// <summary>
        /// Check if the specified ray intersects with anything in the tree. See also: GetColliding.
        /// </summary>
        /// <param name="checkRay">ray to check.</param>
        /// <param name="maxDistance">distance to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool IsColliding(Ray checkRay, double maxDistance)
        {
            return _rootNode.IsColliding(ref checkRay, maxDistance);
        }

        /// <summary>
        /// Returns an array of objects that intersect with the specified bounds, if any. Otherwise returns an empty array. See also: IsColliding.
        /// </summary>
        /// <param name="collidingWith">list to store intersections.</param>
        /// <param name="checkBounds">bounds to check.</param>
        /// <returns>Objects that intersect with the specified bounds.</returns>
        public void GetColliding(List<T> collidingWith, BoundingBox checkBounds)
        {
            _rootNode.GetColliding(ref checkBounds, collidingWith);
        }

        /// <summary>
        /// Returns an array of objects that intersect with the specified ray, if any. Otherwise returns an empty array. See also: IsColliding.
        /// </summary>
        /// <param name="collidingWith">list to store intersections.</param>
        /// <param name="checkRay">ray to check.</param>
        /// <param name="maxDistance">distance to check.</param>
        /// <returns>Objects that intersect with the specified ray.</returns>
        public void GetColliding(List<T> collidingWith, Ray checkRay, double maxDistance = double.PositiveInfinity)
        {
            _rootNode.GetColliding(ref checkRay, collidingWith, maxDistance);
        }

        /// <summary>
        /// Returns an array of objects that intersect with the specified plane, if any. Otherwise returns an empty array.
        /// </summary>
        /// <param name="collidingWith">list to store intersections.</param>
        /// <param name="plane">plane to check.</param>
        /// <param name="maxDistance">distance to check.</param>
        /// <returns>Objects that intersect with the specified plane.</returns>
        public void GetColliding(List<T> collidingWith, Plane plane)
        {
            _rootNode.GetColliding(ref plane, collidingWith);
        }
        public void GetUncollided(Plane plane, bool positiveSide, List<T> uncollided)
        {
            _rootNode.GetUncollided(ref plane, positiveSide, uncollided);
        }

        // #### PRIVATE METHODS ####

        /// <summary>
        /// Grow the octree to fit in all objects.
        /// </summary>
        /// <param name="direction">Direction to grow.</param>
        private void Grow(Point direction)
        {
            int xDirection = direction.X >= 0 ? 1 : -1;
            int yDirection = direction.Y >= 0 ? 1 : -1;
            int zDirection = direction.Z >= 0 ? 1 : -1;
            Node oldRoot = _rootNode;
            double half = _rootNode.BaseLength / 2;
            double newLength = _rootNode.BaseLength * 2;
            Point newCenter = _rootNode.Center + new Point(xDirection * half, yDirection * half, zDirection * half);

            // Create a new, bigger octree root node
            _rootNode = new Node(newLength, _minSize, _looseness, newCenter);

            if (oldRoot.HasAnyObjects())
            {
                // Create 7 new octree children to go with the old root as children of the new root
                int rootPos = _rootNode.BestFitChild(oldRoot.Center);
                Node[] children = new Node[8];
                for (int i = 0; i < 8; i++)
                {
                    if (i == rootPos)
                    {
                        children[i] = oldRoot;
                    }
                    else
                    {
                        xDirection = i % 2 == 0 ? -1 : 1;
                        yDirection = i > 3 ? -1 : 1;
                        zDirection = (i < 2 || (i > 3 && i < 6)) ? -1 : 1;
                        children[i] = new Node(
                            oldRoot.BaseLength,
                            _minSize,
                            _looseness,
                            newCenter + new Point(xDirection * half, yDirection * half, zDirection * half));
                    }
                }

                // Attach the new children to the new root node
                _rootNode.SetChildren(children);
            }
        }

        /// <summary>
        /// Shrink the octree if possible, else leave it the same.
        /// </summary>
        private void Shrink()
        {
            _rootNode = _rootNode.ShrinkIfPossible(_initialSize);
        }
    }
}
﻿// <copyright file="PointOctreeNode.cs">
//     Distributed under the BSD Licence (see LICENCE file).
//     
//     Copyright (c) 2014, Nition, http://www.momentstudio.co.nz/
//     Copyright (c) 2017, Máté Cserép, http://codenet.hu
//     All rights reserved.
// </copyright>
namespace Octree
{
    using System.Collections.Generic;
    using System.Linq;

    public partial class PointOctree<T>
    {
        /// <summary>
        /// A node in a PointOctree
        /// </summary>
        private class Node
        {            
            /// <summary>
            /// Center of this node
            /// </summary>
            public Point Center { get; private set; }

            /// <summary>
            /// Length of the sides of this node
            /// </summary>
            public double SideLength { get; private set; }

            /// <summary>
            /// Minimum size for a node in this octree
            /// </summary>
            private double _minSize;

            /// <summary>
            /// Bounding box that represents this node
            /// </summary>
            private BoundingBox _bounds = default(BoundingBox);

            /// <summary>
            /// Objects in this node
            /// </summary>
            private readonly List<OctreeObject> _objects = new List<OctreeObject>();

            /// <summary>
            /// Child nodes, if any
            /// </summary>
            private Node[] _children = null;

            /// <summary>
            /// Bounds of potential children to this node. These are actual size (with looseness taken into account), not base size
            /// </summary>
            private BoundingBox[] _childBounds;

            /// <summary>
            /// If there are already NumObjectsAllowed in a node, we split it into children
            /// </summary>
            /// <remarks>
            /// A generally good number seems to be something around 8-15
            /// </remarks>
            private const int NumObjectsAllowed = 8;

            /// <summary>
            /// For reverting the bounds size after temporary changes
            /// </summary>
            private Point _actualBoundsSize;

            /// <summary>
            /// Gets a value indicating whether this node has children
            /// </summary>
            private bool HasChildren
            {
                get { return _children != null; }
            }

            /// <summary>
            /// An object in the octree
            /// </summary>
            private class OctreeObject
            {
                /// <summary>
                /// Object content
                /// </summary>
                public T Obj;

                /// <summary>
                /// Object position
                /// </summary>
                public Point Pos;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="baseLengthVal">Length of this node, not taking looseness into account.</param>
            /// <param name="minSizeVal">Minimum size of nodes in this octree.</param>
            /// <param name="centerVal">Center position of this node.</param>
            public Node(double baseLengthVal, double minSizeVal, Point centerVal)
            {
                SetValues(baseLengthVal, minSizeVal, centerVal);
            }

            // #### PUBLIC METHODS ####

            /// <summary>
            /// Add an object.
            /// </summary>
            /// <param name="obj">Object to add.</param>
            /// <param name="objPos">Position of the object.</param>
            /// <returns></returns>
            public bool Add(T obj, Point objPos)
            {
                if (!Encapsulates(_bounds, objPos))
                {
                    return false;
                }
                SubAdd(obj, objPos);
                return true;
            }

            /// <summary>
            /// Remove an object. Makes the assumption that the object only exists once in the tree.
            /// </summary>
            /// <param name="obj">Object to remove.</param>
            /// <returns>True if the object was removed successfully.</returns>
            public bool Remove(T obj)
            {
                bool removed = false;

                for (int i = 0; i < _objects.Count; i++)
                {
                    if (_objects[i].Obj.Equals(obj))
                    {
                        removed = _objects.Remove(_objects[i]);
                        break;
                    }
                }

                if (!removed && _children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        removed = _children[i].Remove(obj);
                        if (removed) break;
                    }
                }

                if (removed && _children != null)
                {
                    // Check if we should merge nodes now that we've removed an item
                    if (ShouldMerge())
                    {
                        Merge();
                    }
                }

                return removed;
            }

            /// <summary>
            /// Removes the specified object at the given position. Makes the assumption that the object only exists once in the tree.
            /// </summary>
            /// <param name="obj">Object to remove.</param>
            /// <param name="objPos">Position of the object.</param>
            /// <returns>True if the object was removed successfully.</returns>
            public bool Remove(T obj, Point objPos)
            {
                if (!Encapsulates(_bounds, objPos))
                {
                    return false;
                }
                return SubRemove(obj, objPos);
            }

            /// <summary>
            /// Return objects that are within <paramref name="maxDistance"/> of the specified ray.
            /// </summary>
            /// <param name="ray">The ray.</param>
            /// <param name="maxDistance">Maximum distance from the ray to consider.</param>
            /// <param name="result">List result.</param>
            /// <returns>Objects within range.</returns>
            public void GetNearby(ref Ray ray, double maxDistance, List<T> result)
            {
                // Does the ray hit this node at all?
                // Note: Expanding the bounds is not exactly the same as a real distance check, but it's fast.
                // TODO: Does someone have a fast AND accurate formula to do this check?
                _bounds.Expand(new Point(maxDistance * 2, maxDistance * 2, maxDistance * 2));
                bool intersected = _bounds.IntersectRay(ray);
                _bounds.Size = _actualBoundsSize;
                if (!intersected)
                {
                    return;
                }

                // Check against any objects in this node
                for (int i = 0; i < _objects.Count; i++)
                {
                    if (SqrDistanceToRay(ray, _objects[i].Pos) <= (maxDistance * maxDistance))
                    {
                        result.Add(_objects[i].Obj);
                    }
                }

                // Check children
                if (_children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        _children[i].GetNearby(ref ray, maxDistance, result);
                    }
                }
            }

            /// <summary>
            /// Return objects that are within <paramref name="maxDistance"/> of the specified position.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="maxDistance">Maximum distance from the position to consider.</param>
            /// <param name="result">List result.</param>
            /// <returns>Objects within range.</returns>
            public void GetNearby(ref Point position, double maxDistance, List<T> result)
            {
                // Does the node contain this position at all?
                // Note: Expanding the bounds is not exactly the same as a real distance check, but it's fast.
                // TODO: Does someone have a fast AND accurate formula to do this check?
                _bounds.Expand(new Point(maxDistance * 2, maxDistance * 2, maxDistance * 2));
                bool contained = _bounds.Contains(position);
                _bounds.Size = _actualBoundsSize;
                if (!contained)
                {
                    return;
                }

                // Check against any objects in this node
                for (int i = 0; i < _objects.Count; i++)
                {
                    if (Point.Distance(position, _objects[i].Pos) <= maxDistance)
                    {
                        result.Add(_objects[i].Obj);
                    }
                }

                // Check children
                if (_children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        _children[i].GetNearby(ref position, maxDistance, result);
                    }
                }
            }

            /// <summary>
            /// Return objects that are within <paramref name="maxDistance"/> of the specified position.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="maxDistance">Maximum distance from the position to consider.</param>
            /// <param name="result">List result.</param>
            /// <returns>Objects within range.</returns>
            public void GetObjectsSplitByPlane(ref Plane plane, List<T> front, List<T> onPlane, List<T> back)
            {
                double distance;
                // Is the plane at least partially in this node?
                if (!_bounds.IntersectPlane(plane, out distance))
                {
                    if (distance > 0) GetAllObjects(front);
                    else GetAllObjects(back);
                    return;
                }

                // Check against any objects in this node
                for (int i = 0; i < _objects.Count; i++)
                {
                    distance = Plane.Distance(plane, _objects[i].Pos);
                    if (distance > 0) front.Add(_objects[i].Obj);
                    else if (distance == 0) onPlane.Add(_objects[i].Obj);
                    else back.Add(_objects[i].Obj);
                }

                // Check children
                if (_children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        _children[i].GetObjectsSplitByPlane(ref plane, front, onPlane, back);
                    }
                }
            }

            /// <summary>
            /// Return all objects in the tree.
            /// </summary>
            /// <returns>All objects.</returns>
            public void GetAll(List<T> result)
            {
                // add directly contained objects
                result.AddRange(_objects.Select(o => o.Obj));

                // add children objects
                if (_children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        _children[i].GetAll(result);
                    }
                }
            }

            /// <summary>
            /// Set the 8 children of this octree.
            /// </summary>
            /// <param name="childOctrees">The 8 new child nodes.</param>
            public void SetChildren(Node[] childOctrees)
            {
                if (childOctrees.Length != 8)
                {
                    throw new System.Exception("Child octree array must be length 8. Was length: " + childOctrees.Length);
                }

                _children = childOctrees;
            }

            /// <summary>
            /// We can shrink the octree if:
            /// - This node is >= double minLength in length
            /// - All objects in the root node are within one octant
            /// - This node doesn't have children, or does but 7/8 children are empty
            /// We can also shrink it if there are no objects left at all!
            /// </summary>
            /// <param name="minLength">Minimum dimensions of a node in this octree.</param>
            /// <returns>The new root, or the existing one if we didn't shrink.</returns>
            public Node ShrinkIfPossible(double minLength)
            {
                if (SideLength < (2 * minLength))
                {
                    return this;
                }
                if (_objects.Count == 0 && (_children == null || _children.Length == 0))
                {
                    return this;
                }

                // Check objects in root
                int bestFit = -1;
                for (int i = 0; i < _objects.Count; i++)
                {
                    OctreeObject curObj = _objects[i];
                    int newBestFit = BestFitChild(curObj.Pos);
                    if (i == 0 || newBestFit == bestFit)
                    {
                        if (bestFit < 0)
                        {
                            bestFit = newBestFit;
                        }
                    }
                    else
                    {
                        return this; // Can't reduce - objects fit in different octants
                    }
                }

                // Check objects in children if there are any
                if (_children != null)
                {
                    bool childHadContent = false;
                    for (int i = 0; i < _children.Length; i++)
                    {
                        if (_children[i].HasAnyObjects())
                        {
                            if (childHadContent)
                            {
                                return this; // Can't shrink - another child had content already
                            }
                            if (bestFit >= 0 && bestFit != i)
                            {
                                return this; // Can't reduce - objects in root are in a different octant to objects in child
                            }
                            childHadContent = true;
                            bestFit = i;
                        }
                    }
                }

                // Can reduce
                if (_children == null)
                {
                    // We don't have any children, so just shrink this node to the new size
                    // We already know that everything will still fit in it
                    SetValues(SideLength / 2, _minSize, _childBounds[bestFit].Center);
                    return this;
                }

                // We have children. Use the appropriate child as the new root node
                return _children[bestFit];
            }

            /// <summary>
            /// Find which child node this object would be most likely to fit in.
            /// </summary>
            /// <param name="objPos">The object's position.</param>
            /// <returns>One of the eight child octants.</returns>
            public int BestFitChild(Point objPos)
            {
                return (objPos.X <= Center.X ? 0 : 1) + (objPos.Y >= Center.Y ? 0 : 4) + (objPos.Z <= Center.Z ? 0 : 2);
            }

            /// <summary>
            /// Checks if this node or anything below it has something in it.
            /// </summary>
            /// <returns>True if this node or any of its children, grandchildren etc have something in them</returns>
            public bool HasAnyObjects()
            {
                if (_objects.Count > 0) return true;

                if (_children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (_children[i].HasAnyObjects()) return true;
                    }
                }

                return false;
            }
            public void GetAllObjects(List<T> objects)
            {
                foreach (var item in _objects)
                {
                    objects.Add(item.Obj);
                }

                if (_children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        _children[i].GetAllObjects(objects);
                    }
                }
            }

            /// <summary>
            /// Returns the squared distance to the given ray from a point.
            /// </summary>
            /// <param name="ray">The ray.</param>
            /// <param name="point">The point to check distance from the ray.</param>
            /// <returns>Squared distance from the point to the closest point of the ray.</returns>
            public static double SqrDistanceToRay(Ray ray, Point point)
            {
                return Point.Cross(ray.Direction, point - ray.Origin).SqrMagnitude;
            }

            // #### PRIVATE METHODS ####

            /// <summary>
            /// Set values for this node. 
            /// </summary>
            /// <param name="baseLengthVal">Length of this node, not taking looseness into account.</param>
            /// <param name="minSizeVal">Minimum size of nodes in this octree.</param>
            /// <param name="centerVal">Centre position of this node.</param>
            private void SetValues(double baseLengthVal, double minSizeVal, Point centerVal)
            {
                SideLength = baseLengthVal;
                _minSize = minSizeVal;
                Center = centerVal;

                // Create the bounding box.
                _actualBoundsSize = new Point(SideLength, SideLength, SideLength);
                _bounds = new BoundingBox(Center, _actualBoundsSize);

                double quarter = SideLength / 4f;
                double childActualLength = SideLength / 2;
                Point childActualSize = new Point(childActualLength, childActualLength, childActualLength);
                _childBounds = new BoundingBox[8];
                _childBounds[0] = new BoundingBox(Center + new Point(-quarter, quarter, -quarter), childActualSize);
                _childBounds[1] = new BoundingBox(Center + new Point(quarter, quarter, -quarter), childActualSize);
                _childBounds[2] = new BoundingBox(Center + new Point(-quarter, quarter, quarter), childActualSize);
                _childBounds[3] = new BoundingBox(Center + new Point(quarter, quarter, quarter), childActualSize);
                _childBounds[4] = new BoundingBox(Center + new Point(-quarter, -quarter, -quarter), childActualSize);
                _childBounds[5] = new BoundingBox(Center + new Point(quarter, -quarter, -quarter), childActualSize);
                _childBounds[6] = new BoundingBox(Center + new Point(-quarter, -quarter, quarter), childActualSize);
                _childBounds[7] = new BoundingBox(Center + new Point(quarter, -quarter, quarter), childActualSize);
            }

            /// <summary>
            /// Private counterpart to the public Add method.
            /// </summary>
            /// <param name="obj">Object to add.</param>
            /// <param name="objPos">Position of the object.</param>
            private void SubAdd(T obj, Point objPos)
            {
                // We know it fits at this level if we've got this far

                // We always put things in the deepest possible child
                // So we can skip checks and simply move down if there are children aleady
                if (!HasChildren)
                {
                    // Just add if few objects are here, or children would be below min size
                    if (_objects.Count < NumObjectsAllowed || (SideLength / 2) < _minSize)
                    {
                        OctreeObject newObj = new OctreeObject { Obj = obj, Pos = objPos };
                        _objects.Add(newObj);
                        return; // We're done. No children yet
                    }

                    // Enough objects in this node already: Create the 8 children
                    int bestFitChild;
                    if (_children == null)
                    {
                        Split();
                        if (_children == null)
                        {
                            throw new System.Exception("Child creation failed for an unknown reason. Early exit.");
                        }

                        // Now that we have the new children, move this node's existing objects into them
                        for (int i = _objects.Count - 1; i >= 0; i--)
                        {
                            OctreeObject existingObj = _objects[i];
                            // Find which child the object is closest to based on where the
                            // object's center is located in relation to the octree's center
                            bestFitChild = BestFitChild(existingObj.Pos);
                            _children[bestFitChild].SubAdd(existingObj.Obj, existingObj.Pos); // Go a level deeper					
                            _objects.Remove(existingObj); // Remove from here
                        }
                    }
                }

                // Handle the new object we're adding now
                int bestFit = BestFitChild(objPos);
                _children[bestFit].SubAdd(obj, objPos);
            }

            /// <summary>
            /// Private counterpart to the public <see cref="Remove(T, Point)"/> method.
            /// </summary>
            /// <param name="obj">Object to remove.</param>
            /// <param name="objPos">Position of the object.</param>
            /// <returns>True if the object was removed successfully.</returns>
            private bool SubRemove(T obj, Point objPos)
            {
                bool removed = false;

                for (int i = 0; i < _objects.Count; i++)
                {
                    if (_objects[i].Obj.Equals(obj))
                    {
                        removed = _objects.Remove(_objects[i]);
                        break;
                    }
                }

                if (!removed && _children != null)
                {
                    int bestFitChild = BestFitChild(objPos);
                    removed = _children[bestFitChild].SubRemove(obj, objPos);
                }

                if (removed && _children != null)
                {
                    // Check if we should merge nodes now that we've removed an item
                    if (ShouldMerge())
                    {
                        Merge();
                    }
                }

                return removed;
            }

            /// <summary>
            /// Splits the octree into eight children.
            /// </summary>
            private void Split()
            {
                double quarter = SideLength / 4f;
                double newLength = SideLength / 2;
                _children = new Node[8];
                _children[0] = new Node(newLength, _minSize, Center + new Point(-quarter, quarter, -quarter));
                _children[1] = new Node(newLength, _minSize, Center + new Point(quarter, quarter, -quarter));
                _children[2] = new Node(newLength, _minSize, Center + new Point(-quarter, quarter, quarter));
                _children[3] = new Node(newLength, _minSize, Center + new Point(quarter, quarter, quarter));
                _children[4] = new Node(newLength, _minSize, Center + new Point(-quarter, -quarter, -quarter));
                _children[5] = new Node(newLength, _minSize, Center + new Point(quarter, -quarter, -quarter));
                _children[6] = new Node(newLength, _minSize, Center + new Point(-quarter, -quarter, quarter));
                _children[7] = new Node(newLength, _minSize, Center + new Point(quarter, -quarter, quarter));
            }

            /// <summary>
            /// Merge all children into this node - the opposite of Split.
            /// Note: We only have to check one level down since a merge will never happen if the children already have children,
            /// since THAT won't happen unless there are already too many objects to merge.
            /// </summary>
            private void Merge()
            {
                // Note: We know children != null or we wouldn't be merging
                for (int i = 0; i < 8; i++)
                {
                    Node curChild = _children[i];
                    int numObjects = curChild._objects.Count;
                    for (int j = numObjects - 1; j >= 0; j--)
                    {
                        OctreeObject curObj = curChild._objects[j];
                        _objects.Add(curObj);
                    }
                }
                // Remove the child nodes (and the objects in them - they've been added elsewhere now)
                _children = null;
            }

            /// <summary>
            /// Checks if outerBounds encapsulates the given point.
            /// </summary>
            /// <param name="outerBounds">Outer bounds.</param>
            /// <param name="point">Point.</param>
            /// <returns>True if innerBounds is fully encapsulated by outerBounds.</returns>
            private static bool Encapsulates(BoundingBox outerBounds, Point point)
            {
                return outerBounds.Contains(point);
            }

            /// <summary>
            /// Checks if there are few enough objects in this node and its children that the children should all be merged into this.
            /// </summary>
            /// <returns>True there are less or the same amount of objects in this and its children than <see cref="NumObjectsAllowed"/>.</returns>
            private bool ShouldMerge()
            {
                int totalObjects = _objects.Count;
                if (_children != null)
                {
                    foreach (Node child in _children)
                    {
                        if (child._children != null)
                        {
                            // If any of the *children* have children, there are definitely too many to merge,
                            // or the child would have been merged already
                            return false;
                        }
                        totalObjects += child._objects.Count;
                    }
                }
                return totalObjects <= NumObjectsAllowed;
            }
        }
    }
}
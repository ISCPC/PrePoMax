using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public class ContactSurface
    {
        // Variables                                                                                                                
        public FeMesh Mesh;
        public BasePart Part;
        public int Id;


        // Constructors                                                                                                             
        public ContactSurface(FeMesh mesh, BasePart part, int id)
        {
            Mesh = mesh;
            Part = part;
            Id = id;
        }
    }
    public class ContactSearch
    {
        // Variables                                                                                                                
        private FeMesh _mesh;


        // Constructors                                                                                                             
        public ContactSearch(FeMesh mesh) 
        {
            _mesh = mesh;
        }


        // Methods                                                                                                                  
        public List<ContactSurface[]> FindContactPairs(double distance, double angleDeg)
        {
            int count = 0;
            foreach (var partEntry in _mesh.Parts) count += partEntry.Value.Visualization.FaceAreas.Length;
            //
            ContactSurface[] contactSurfaces = new ContactSurface[count];
            count = 0;
            //
            foreach (var partEntry in _mesh.Parts)
            {
                for (int i = 0; i < partEntry.Value.Visualization.FaceAreas.Length; i++)
                {
                    contactSurfaces[count++] = new ContactSurface(_mesh, partEntry.Value, i);
                }
            }
            //
            double angleRad = angleDeg * Math.PI / 180;
            List<ContactSurface[]> contactPairs = new List<ContactSurface[]>();
            for (int i = 0; i < contactSurfaces.Length; i++)
            {
                for (int j = i + 1; j < contactSurfaces.Length; j++)
                {
                    if (CheckSurfaceToSurfaceContact(contactSurfaces[i], contactSurfaces[j], distance, angleRad))
                    {
                        contactPairs.Add(new ContactSurface[] { contactSurfaces[i], contactSurfaces[j] });
                    }
                }
            }
            //
            return contactPairs;
        }
        private bool CheckSurfaceToSurfaceContact(ContactSurface cs1, ContactSurface cs2, double distance, double angleRad)
        {
            double dist;
            double ang;
            int[] cell1;
            int[] cell2;
            double[] p = new double[3];
            double[] q = new double[3];
            double[] a = new double[3];
            double[] b = new double[3];
            double[] n1 = new double[3];
            double[] n2 = new double[3];
            double[][] t1 = new double[3][];
            double[][] t2 = new double[3][];
            //            
            foreach (int cell1Id in cs1.Part.Visualization.CellIdsByFace[cs1.Id])
            {
                cell1 = cs1.Part.Visualization.Cells[cell1Id];
                t1[0] = cs1.Mesh.Nodes[cell1[0]].Coor;
                t1[1] = cs1.Mesh.Nodes[cell1[1]].Coor;
                t1[2] = cs1.Mesh.Nodes[cell1[2]].Coor;
                //
                Geometry.VmV(ref a, t1[1], t1[0]);
                Geometry.VmV(ref b, t1[2], t1[0]);
                Geometry.VcrossV(ref n1, a, b);
                Geometry.VxS(ref n1, n1, 1 / Math.Sqrt(Geometry.VdotV(n1, n1)));
                //
                foreach (int cell2Id in cs2.Part.Visualization.CellIdsByFace[cs2.Id])
                {
                    cell2 = cs2.Part.Visualization.Cells[cell2Id];
                    t2[0] = cs2.Mesh.Nodes[cell2[0]].Coor;
                    t2[1] = cs2.Mesh.Nodes[cell2[1]].Coor;
                    t2[2] = cs2.Mesh.Nodes[cell2[2]].Coor;
                    //
                    Geometry.VmV(ref a, t2[1], t2[0]);
                    Geometry.VmV(ref b, t2[2], t2[0]);
                    Geometry.VcrossV(ref n2, a, b);
                    Geometry.VxS(ref n2, n2, 1 / Math.Sqrt(Geometry.VdotV(n2, n2)));
                    //
                    ang = Math.Acos(Geometry.VdotV(n1, n2));
                    //
                    if (ang > angleRad)
                    {
                        dist = Geometry.TriDist(ref p, ref q, t1, t2);
                        //
                        if (dist < distance) return true;
                    }
                }
            }
            return false;
        }
    }
}

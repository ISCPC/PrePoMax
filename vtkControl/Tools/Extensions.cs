using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace vtkControl
{
    public static class Extensions
    {
        public static void Subtract(this vtkSelection selection1, vtkSelection selection2)
        {
            for (uint n = 0; n < selection2.GetNumberOfNodes(); ++n)
            {
                selection1.Subtract(selection2.GetNode(n));
            }
        }
        public static void Subtract(this vtkSelection selection, vtkSelectionNode node)
        {
            bool subtracted = false;
            for (uint tn = 0; tn < selection.GetNumberOfNodes(); ++tn)
            {
                vtkSelectionNode tnode = selection.GetNode(tn);

                if (tnode.EqualProperties(node, true))
                {
                    tnode.SubtractSelectionList(node);
                    subtracted = true;
                }
            }
            if (!subtracted)
            {
                //vtkErrorMacro("Could not subtract selections");
            }
        }
        public static void SubtractSelectionList(this vtkSelectionNode node1, vtkSelectionNode node2)
        {
            vtkSelectionContent type = (vtkSelectionContent)node1.GetContentType(); //vtkSelectionContent
            switch (type)
            {
                case vtkSelectionContent.GLOBALIDS:
                case vtkSelectionContent.INDICES:
                case vtkSelectionContent.PEDIGREEIDS:
                    {
                        vtkDataSetAttributes fd1 = node1.GetSelectionData();
                        vtkDataSetAttributes fd2 = node2.GetSelectionData();

                        if(fd1.GetNumberOfArrays() != fd2.GetNumberOfArrays()) return;
                        if( fd1.GetNumberOfArrays() != 1 || fd2.GetNumberOfArrays() != 1) return;
                        if (fd1.GetArray(0).GetDataType() != 12 || fd2.GetArray(0).GetDataType() != 12) return; // 12 ... VTK_ID_TYPE

                        vtkIdTypeArray fd1_array = (vtkIdTypeArray)fd1.GetArray(0);
                        vtkIdTypeArray fd2_array = (vtkIdTypeArray)fd2.GetArray(0);

                        long fd1_N = fd1_array.GetNumberOfTuples();
                        long fd2_N = fd2_array.GetNumberOfTuples();

                        List<long> fd1_list = new List<long>();
                        for (int i = 0; i < fd1_N; i++) fd1_list.Add(fd1_array.GetValue(i));

                        List<long> fd2_list = new List<long>();
                        for (int i = 0; i < fd2_N; i++) fd2_list.Add(fd2_array.GetValue(i));

                        fd1_list.Sort();
                        fd2_list.Sort();

                        foreach (long value in fd2_list)
                        {
                            if (fd1_list.Contains(value)) fd1_list.Remove(value);
                        }

                        fd1_array.Reset();
                        foreach (long value in fd1_list)
                        {
                            fd1_array.InsertNextValue(value);
                        }

                        break;
                    }
                case vtkSelectionContent.BLOCKS:
                case vtkSelectionContent.FRUSTUM:
                case vtkSelectionContent.LOCATIONS:
                case vtkSelectionContent.THRESHOLDS:
                case vtkSelectionContent.VALUES:
                default:
                    break;
            }
        }


        public static void Subtract(this HashSet<int> items, int[] itemsToRemove)
        {
            for (int i = 0; i < itemsToRemove.Length; i++)
            {
                items.Remove(itemsToRemove[i]);
            }
        }

       
    }
}

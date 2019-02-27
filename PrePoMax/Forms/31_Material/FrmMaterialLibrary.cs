using CaeGlobals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeModel;
using Newtonsoft.Json;

namespace PrePoMax.Forms
{
    public partial class FrmMaterialLibrary : Form
    {
        // Variables                                                                                                                
        private Controller _controller;
        private bool _libraryChanged;
        private bool _modelChanged;

        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmMaterialLibrary(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _libraryChanged = false;
            _modelChanged = false;
        }


        // Event handlers                                                                                                           
        private void FrmMaterialLibrary_Load(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model != null)
                {
                    ListViewItem item;
                    foreach (var entry in _controller.Model.Materials)
                    {
                        item = lvModelMaterials.Items.Add(entry.Value.Name);
                        item.Name = entry.Value.Name;
                        item.Tag = entry.Value; // do not clone, to determine if the material changed
                    }
                    if (lvModelMaterials.Items.Count > 0) lvModelMaterials.Items[0].Selected = true;
                }

                LoadLibraryFromFile();

                TreeNode materialNode;
                GetNodeContainingFirstMaterial(btvLibrary.Nodes[0], out materialNode);
                if (materialNode != null) btvLibrary.SelectedNode = materialNode;
                else btvLibrary.SelectedNode = btvLibrary.Nodes[0];
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void btvLibrary_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (btvLibrary.SelectedNode != null) tbCategoryName.Text = btvLibrary.SelectedNode.Text;
        }
        private void btvLibrary_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (btvLibrary.HitTest(e.Location).Node == null)
                {
                    //btvLibrary.SelectedNode = null;
                }
            }
            catch
            { }
        }
        private void btvLibrary_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void lvModelMaterials_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (lvModelMaterials.HitTest(e.Location).Item == null)
                {
                    //lvModelMaterials.SelectedItems.Clear();
                }
            }
            catch
            { }
        }
        private void lvModelMaterials_Leave(object sender, EventArgs e)
        {
            if (lvModelMaterials.SelectedItems.Count == 1)
            {
                lvModelMaterials.SelectedItems[0].BackColor = SystemColors.Highlight;
                lvModelMaterials.SelectedItems[0].ForeColor = Color.White;
            }
        }
        private void lvModelMaterials_Enter(object sender, EventArgs e)
        {
            if (lvModelMaterials.SelectedItems.Count == 1)
            {
                lvModelMaterials.SelectedItems[0].BackColor = Color.White;
                lvModelMaterials.SelectedItems[0].ForeColor = Color.Black;
            }
        }

        private void tbCategoryName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnRename_Click(null, null);
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode parentNode;
                if (btvLibrary.SelectedNode != null) parentNode = btvLibrary.SelectedNode;
                else parentNode = btvLibrary.Nodes[0];

                if (parentNode.Tag == null)
                {
                    TreeNode node = parentNode.Nodes.Add("NewCategory");
                    node.Name = "NewCategory";

                    parentNode.Expand();
                    btvLibrary.SelectedNode = node;
                    ApplyFormatingRecursive(node);
                    btvLibrary.Focus();

                    tbCategoryName.Focus();

                    _libraryChanged = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnDeleteFromLibrary_Click(object sender, EventArgs e)
        {
            if (btvLibrary.SelectedNode != null && btvLibrary.SelectedNode.Parent != null)
            {
                btvLibrary.SelectedNode.Parent.Nodes.Remove(btvLibrary.SelectedNode);
                _libraryChanged = true;
            }
        }
        private void btnRename_Click(object sender, EventArgs e)
        {
            try
            {
                if (btvLibrary.SelectedNode != null && btvLibrary.SelectedNode.Text != tbCategoryName.Text && btvLibrary.SelectedNode.Parent != null)
                {
                    if (!btvLibrary.SelectedNode.Parent.Nodes.ContainsKey(tbCategoryName.Text))
                    {
                        Material test = new Material(tbCategoryName.Text); // test the name
                        btvLibrary.SelectedNode.Text = tbCategoryName.Text;
                        btvLibrary.SelectedNode.Name = tbCategoryName.Text;
                        if (btvLibrary.SelectedNode.Tag != null) ((Material)btvLibrary.SelectedNode.Tag).Name = tbCategoryName.Text;

                        _libraryChanged = true;
                    }
                    else throw new CaeException("The node '" + btvLibrary.SelectedNode.Parent.Text + "' already contains the node named '" + tbCategoryName.Text + "'.");
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void btnDeleteFromModel_Click(object sender, EventArgs e)
        {
            if (lvModelMaterials.SelectedItems.Count == 1)
            {
                lvModelMaterials.Items.Remove(lvModelMaterials.SelectedItems[0]);
                _modelChanged = true;
            }
        }

        private void btnCopyToLibrary_Click(object sender, EventArgs e)
        {
            try
            {

                if (lvModelMaterials.SelectedItems.Count == 1 && btvLibrary.SelectedNode != null)
                {
                    TreeNode categoryNode;
                    if (btvLibrary.SelectedNode.Tag == null) categoryNode = btvLibrary.SelectedNode;    // Category
                    else categoryNode = btvLibrary.SelectedNode.Parent;                                 // Material

                    ListViewItem modelMaterial = lvModelMaterials.SelectedItems[0];

                    if (!categoryNode.Nodes.ContainsKey(modelMaterial.Text))
                    {
                        TreeNode newMaterialNode = categoryNode.Nodes.Add(modelMaterial.Text);
                        newMaterialNode.Name = modelMaterial.Text;
                        newMaterialNode.Tag = modelMaterial.Tag.DeepClone(); ;

                        categoryNode.Expand();
                        btvLibrary.SelectedNode = newMaterialNode;

                        _libraryChanged = true;
                    }
                    else throw new CaeException("The node '" + categoryNode.Text + "' already contains the node named '" + modelMaterial.Text + "'.");
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnCopyToModel_Click(object sender, EventArgs e)
        {
            try
            {
                if (btvLibrary.SelectedNode != null && btvLibrary.SelectedNode.Tag != null)
                {
                    if (!lvModelMaterials.Items.ContainsKey(btvLibrary.SelectedNode.Text))
                    {
                        ListViewItem modelMaterial = lvModelMaterials.Items.Add(btvLibrary.SelectedNode.Text);
                        modelMaterial.Name = modelMaterial.Text;
                        modelMaterial.Tag = btvLibrary.SelectedNode.Tag.DeepClone();

                        lvModelMaterials_Enter(null, null);
                        modelMaterial.Selected = true;
                        lvModelMaterials_Leave(null, null);

                        _modelChanged = true;
                    }
                    else throw new CaeException("The model already contains the material named '" + btvLibrary.SelectedNode.Text + "'.");
                }
                else throw new CaeException("Please select the material in the library materials to be copied to the model materials.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveLibraryToFile();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model != null && _modelChanged)
                {
                    List<Material> newMaterials = new List<Material>();
                    List<Material> changedMaterials = new List<Material>();
                    List<string> deletedMaterials = new List<string>();
                    Material material;

                    foreach (ListViewItem item in lvModelMaterials.Items)
                    {
                        if (_controller.Model.Materials.TryGetValue(item.Name, out material))
                        {
                            if ((Material)item.Tag != material) changedMaterials.Add((Material)item.Tag);
                        }
                        else newMaterials.Add((Material)item.Tag);
                    }

                    foreach (var entry in _controller.Model.Materials)
                    {
                        if (!lvModelMaterials.Items.ContainsKey(entry.Key)) deletedMaterials.Add(entry.Key);
                    }

                    if (deletedMaterials.Count > 0) _controller.RemoveMaterialsCommand(deletedMaterials.ToArray());
                    foreach (Material changedMaterial in changedMaterials) _controller.ReplaceMaterialCommand(changedMaterial.Name, changedMaterial);
                    foreach (Material newMaterial in newMaterials) _controller.AddMaterialCommand(newMaterial);
                }
                Close();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void FrmMaterialLibrary_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == CloseReason.UserClosing && _libraryChanged)
                {
                    DialogResult response = MessageBox.Show("Save material library before closing?", "Warning", MessageBoxButtons.YesNoCancel);
                    if (response == DialogResult.Yes) SaveLibraryToFile();
                    else if (response == DialogResult.Cancel) e.Cancel = true;
                }
            }
            catch
            { }
        }


        // Methods                                                                                                                  
        private void SaveLibraryToFile()
        {
            string fileName = Path.Combine(Application.StartupPath, Globals.MaterialLibraryFileName);

            MaterialLibraryItem materials = new MaterialLibraryItem(btvLibrary.Nodes[0].Text);
            TreeNodesToItemList(btvLibrary.Nodes[0], materials);

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string json = JsonConvert.SerializeObject(materials, Formatting.Indented, settings);
            File.WriteAllText(fileName, json);

            _libraryChanged = false;
        }
        private void TreeNodesToItemList(TreeNode node, MaterialLibraryItem item)
        {
            item.Expanded = node.IsExpanded;

            foreach (TreeNode childNode in node.Nodes)
            {
                MaterialLibraryItem childItem = new MaterialLibraryItem(childNode.Name);
                if (childNode.Tag != null) childItem.Tag = (Material)childNode.Tag.DeepClone();
                item.Items.Add(childItem);

                TreeNodesToItemList(childNode, childItem);
            }
        }

        private void LoadLibraryFromFile()
        {
            string fileName = Path.Combine(Application.StartupPath, Globals.MaterialLibraryFileName);

            if (File.Exists(fileName))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                MaterialLibraryItem mli = JsonConvert.DeserializeObject<MaterialLibraryItem>(File.ReadAllText(fileName), settings);

                btvLibrary.Nodes[0].Nodes.Clear();
                ItemListToTreeNodes(mli, btvLibrary.Nodes[0]);
            }

            ApplyFormatingRecursive(btvLibrary.Nodes[0]);
        }
        private void ItemListToTreeNodes(MaterialLibraryItem item, TreeNode node)
        {
            TreeNode childNode;
            foreach (MaterialLibraryItem childItem in item.Items)
            {
                childNode = node.Nodes.Add(childItem.Name);
                childNode.Name = childItem.Name;

                if (childItem.Tag != null) childNode.Tag = childItem.Tag.DeepClone();
                else ItemListToTreeNodes(childItem, childNode);
            }

            if (item.Expanded) node.Expand();
        }

        private void ApplyFormatingRecursive(TreeNode node)
        {
            if (node.Tag == null) node.ForeColor = SystemColors.Highlight;
            else node.ForeColor = Color.Black;

            foreach (TreeNode childNode in node.Nodes)
            {
                ApplyFormatingRecursive(childNode);
            }
        }

        private void GetNodeContainingFirstMaterial(TreeNode node, out TreeNode firstNodeWithMaterial)
        {
            firstNodeWithMaterial = null;

            if (node.Tag != null)
            {
                firstNodeWithMaterial = node;
                return;
            }

            foreach (TreeNode childNode in node.Nodes)
            {
                GetNodeContainingFirstMaterial(childNode, out firstNodeWithMaterial);
                if (firstNodeWithMaterial != null) return;
            }
        }




    }
}

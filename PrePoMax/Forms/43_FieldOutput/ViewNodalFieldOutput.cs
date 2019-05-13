using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    [EnumResource("PrePoMax.Properties.Resources")]
    [Editor(typeof(StandardValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [Flags]
    public enum ViewNodalFieldVariable
    {
        // must start at 1 for the UI to work
        [StandardValue("RF", Description = "Reaction forces.")]
        RF = 1,

        [StandardValue("U", Description = "Displacements.")]
        U = 2
    }

    [Serializable]
    //[ClassResource(BaseName = "PrePoMax.Properties.Resources", KeyPrefix = "ViewNodalFieldOutput_")]
    public class ViewNodalFieldOutput : ViewFieldOutput
    {
        // Variables                                                                                                                
        private CaeModel.NodalFieldOutput _fieldOutput;
        private DynamicCustomTypeDescriptor _dctd = null;

        // Properties                                                                                                               
        public override string Name { get { return _fieldOutput.Name; } set { _fieldOutput.Name = value; } }
     
        [OrderedDisplayName(1, 10, "Variables to output")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Nodal field variables")]
        public ViewNodalFieldVariable Variables 
        { 
            get
            { 
                return (ViewNodalFieldVariable)_fieldOutput.Variables; 
            } 
            set
            { 
                _fieldOutput.Variables = (CaeModel.NodalFieldVariable)value;
            } 
        }

        public override CaeModel.FieldOutput Base { get { return _fieldOutput; } set { _fieldOutput = (CaeModel.NodalFieldOutput)value; } }


        // Constructors                                                                                                             
        public ViewNodalFieldOutput(CaeModel.NodalFieldOutput fieldOutput)
        {
            _fieldOutput = fieldOutput;
            _dctd = ProviderInstaller.Install(this);
        }
    }



   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class StepCollection
    {
        // Variables                                                                                                                
        private Dictionary<string, Step> _steps;
        

        // Properties                                                                                                               
        public Dictionary<string, Step> Steps { get { return _steps; } }
        

        // Constructors                                                                                                             
        public StepCollection()
        {
            _steps = new Dictionary<string, Step>();

            InitialStep initial = new InitialStep("Initial");
            //AddStep(initial);
        }

        // Methods                                                                                                                         
        public void AddStep(Step step)
        {
            if (_steps.Count >= 1)
            {
                Step lastStep = _steps.Last().Value;
                foreach (var entry in lastStep.BoundaryConditions)
                {
                    step.AddBoundaryCondition(entry.Value.DeepClone());
                }
                foreach (var entry in lastStep.Loads)
                {
                    step.AddLoad(entry.Value.DeepClone());
                }
            }
            _steps.Add(step.Name, step);
        }
        public void ReplaceStep(string oldStepName, Step newStep)
        {
            Dictionary<string, Step> newSteps = new Dictionary<string, Step>();
            foreach (var entry in _steps)
            {
                if (entry.Key == oldStepName)
                {
                    newSteps.Add(newStep.Name, newStep);
                }
                else newSteps.Add(entry.Key, entry.Value);
            }
            _steps = newSteps;
        }
        public void AddBoundaryCondition(BoundaryCondition boundaryCondition, string stepName)
        {
            foreach (var entry in _steps)
            {
                if (entry.Value.Name == stepName)
                {
                    entry.Value.AddBoundaryCondition(boundaryCondition);
                }
            }
        }
        public void AddLoad(Load load, string stepName)
        {
            bool add = false;
            foreach (var entry in _steps)
            {
                if (entry.Value.Name == stepName) add = true;

                if (add)
                {
                    entry.Value.AddLoad(load);
                }
            }
        }
        public void AddFieldOutput(FieldOutput fieldOutput, string stepName)
        {
            bool add = false;
            foreach (var entry in _steps)
            {
                if (entry.Value.Name == stepName) add = true;

                if (add)
                {
                    entry.Value.AddFieldOutput(fieldOutput);
                }
            }
        }
        public string[] GetFieldOutputNames()
        {
            List<string> names = new List<string>();
            foreach (var entry in _steps)
            {
                foreach (var field in entry.Value.FieldOutputs)
                {
                    names.Add(entry.Key + " -> " + field.Key);
                }
            }
            return names.ToArray();
        }
    }
}

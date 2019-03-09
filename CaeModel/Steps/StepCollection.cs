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
        private List<Step> _steps;


        // Properties                                                                                                               
        public List<Step> StepsList { get { return _steps; } }


        // Constructors                                                                                                             
        public StepCollection()
        {
            _steps = new List<Step>();

            //InitialStep initial = new InitialStep("Initial");
            //AddStep(initial);
        }

        // Methods                                                                                                                         
        public void AddStep(Step step)
        {
            if (_steps.Count >= 1)
            {
                Step lastStep = _steps.Last();
                foreach (var entry in lastStep.BoundaryConditions)
                {
                    step.AddBoundaryCondition(entry.Value.DeepClone());
                }
                foreach (var entry in lastStep.Loads)
                {
                    if (step.SupportsLoads) step.AddLoad(entry.Value.DeepClone());
                }
            }
            _steps.Add(step);
        }
        public Step GetStep(string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName) return step;
            }
            return null;
        }
        public string[] GetStepNames()
        {
            List<string> names = new List<string>();
            foreach (var step in _steps) names.Add(step.Name);
            return names.ToArray();
        }
        public void ReplaceStep(string oldStepName, Step newStep)
        {
            List<Step> newSteps = new List<Step>();
            foreach (var step in _steps)
            {
                if (step.Name == oldStepName) newSteps.Add(newStep);
                else newSteps.Add(step);
            }
            _steps = newSteps;
        }
        public void RemoveStep(string stepName)
        {
            Step stepToRemove = null;
            foreach (var step in _steps)
            {
                if (step.Name == stepName)
                {
                    stepToRemove = step;
                    break;
                }
            }
            if (stepToRemove != null) _steps.Remove(stepToRemove);

        }
        public void AddBoundaryCondition(BoundaryCondition boundaryCondition, string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName) step.AddBoundaryCondition(boundaryCondition);
            }
        }
        public void AddLoad(Load load, string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName)  step.AddLoad(load);
            }
        }
        public void AddFieldOutput(FieldOutput fieldOutput, string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName) step.AddFieldOutput(fieldOutput);
            }
        }
        public string[] GetFieldOutputNames()
        {
            List<string> names = new List<string>();
            foreach (var step in _steps)
            {
                foreach (var field in step.FieldOutputs) names.Add(step.Name + " -> " + field.Key);
            }
            return names.ToArray();
        }
    }
}

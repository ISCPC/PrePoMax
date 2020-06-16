using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserControls
{
    [Serializable]
    public class AdvisorItemLinkLabel : AdvisorItem
    {
        // Variables                                                                                                                
        protected List<Action<object, EventArgs>> _actions;
        protected List<object> _senders;
        protected List<EventArgs> _eventArgs;


        // Properties                                                                                                               
        //public List<Action<object, EventArgs>> Actions { get { return _actions; } set { _actions = value; } }


        // Constructors                                                                                                             
        public AdvisorItemLinkLabel()
        {
            _actions = new List<Action<object, EventArgs>>();
            _senders = new List<object>();
            _eventArgs = new List<EventArgs>();
        }

        // Methods                                                                                                                  
        public void AddAction(Action<object, EventArgs> action)
        {
            AddAction(action, null, null);
        }
        public void AddAction(Action<object, EventArgs> action, object sender, EventArgs e)
        {
            _actions.Add(action);
            _senders.Add(sender);
            _eventArgs.Add(e);
        }
        public void Activate()
        {
            for (int i = 0; i < _actions.Count; i++) _actions[i](_senders[i], _eventArgs[i]);
        }
    }
}

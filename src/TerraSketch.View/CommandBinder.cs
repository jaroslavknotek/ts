using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using TerraSketch.Presenters.Interfaces;

namespace TerraSketch.View
{


    public class CommandBinder
    {
       

        private BindingList bindings = new BindingList();


        public CommandBinder()
        {
            Application.Idle += (s,e)=> Application_Idle(s,e).Wait();
        }

        private async Task Application_Idle(object sender, EventArgs e)
        {


            foreach (var bind in bindings)
            {
                var canCall = bind.BingingItem.CanCallCommand();

                if (canCall != bind.LastCanCallResult)
                {
                    bind.BingingItem.OnCanExecuteChanged();
                    bind.LastCanCallResult = canCall;
                }


                if (canCall)
                    await bind.BingingItem.Exec();

            }
        }

        private async Task execIfCan(IBindingItem bi)
        {
            if (bi.CanCallCommand())
            {
                await bi.Exec();
            }
        }

        public void Bind(ICommandWrapper c, Keys key)
        {

            var bi = new CommandToKeyBindingItem(c, key);
            bindings.Add(bi);
        }
        public void Bind(ICommandWrapper c, params Keys[] k)
        {
            var bi = new CommandToKeyBindingItem(c, k);
            bindings.Add(bi);
        }
        
        #region MouseOnControlBinding
        IDictionary<Control, ActionsPerControl> _actionPerControl = new Dictionary<Control,ActionsPerControl>();
        private class ActionsPerControl
        {
            IDictionary<MouseButtons, SortedList<ModifierKeys, IBindingItem>> _clickActions = new Dictionary<MouseButtons, SortedList<ModifierKeys, IBindingItem>>();
            IDictionary<MouseButtons, SortedList<ModifierKeys, IBindingItem>> _doubleClickActions = new Dictionary<MouseButtons, SortedList<ModifierKeys, IBindingItem>>();
            public void AddOnDoubleClickAction(MouseButtons button, IBindingItem cmd, ModifierKeys key)
            {
                add(button, cmd, _doubleClickActions,key);
            }
            public void AddOnClickAction(MouseButtons button, IBindingItem cmd, ModifierKeys key)
            {
                add(button, cmd, _clickActions,key);
            }
           
          
            public async Task OnDoubleClickAction(MouseButtons button)
            {
               await react(button, _doubleClickActions);
            }

            public async Task OnClickAction(MouseButtons button)
            {
                await react(button,_clickActions);
            }

            private async Task react(MouseButtons button, IDictionary<MouseButtons, SortedList<ModifierKeys, IBindingItem>> ac)
            {
                SortedList<ModifierKeys, IBindingItem> list;
                if (!ac.TryGetValue(button, out list)) return;

                foreach (var item in list.Values)
                {
                    if (item.CanCallCommand())
                    {
                        await item.Exec();
                        break;
                    }
                }
            }

            private void add(MouseButtons button, IBindingItem cmd, IDictionary<MouseButtons, SortedList<ModifierKeys, IBindingItem>> ac,ModifierKeys keys)
            {
                SortedList<ModifierKeys,IBindingItem> list;
                if (!ac.TryGetValue(button, out list))
                {
                    list = new SortedList< ModifierKeys,IBindingItem>( new ModifierComparer() );
                    ac.Add(button, list);
                }
                list.Add(keys ,cmd);
            }

            private class ModifierComparer : IComparer<ModifierKeys>
            {
                public int Compare(ModifierKeys x, ModifierKeys y)
                {
                    if (x == y) return 0;
                    if (x > y) return -1;
                    return 1;

                }
            }
        }
        public enum ModifierKeys
        {
            None = 0, Control = 1, Shift = 2,ControlShift = 3
        }
        public void Bind(ICommandWrapper cmd, Control cont, Mouse mouseBtn, ModifierKeys key = ModifierKeys.None)
        {
            ActionsPerControl o = getOrCreateActionContainer(cont);
            CommandToKeyBindingItem kbi;
            
                
            if(key == ModifierKeys.Control)
                kbi = new CommandToKeyBindingItem(cmd, Keys.ControlKey);
            else if (key == ModifierKeys.Shift)
                kbi = new CommandToKeyBindingItem(cmd, Keys.ShiftKey);
            else if (key == ModifierKeys.ControlShift)
                kbi = new CommandToKeyBindingItem(cmd, Keys.ControlKey, Keys.ShiftKey);
            else
                kbi = new CommandToKeyBindingItem(cmd);

            if (mouseBtn == Mouse.LeftDoubleclick)
                o.AddOnDoubleClickAction(MouseButtons.Left, kbi,key);
            else if (mouseBtn == Mouse.LeftDown)
                o.AddOnClickAction(MouseButtons.Left, kbi,key);
            else if (mouseBtn == Mouse.RightDown)
                o.AddOnClickAction(MouseButtons.Right, kbi,key);
        }
       

      

        

        private void Cont_MouseClickAction(object sender, MouseEventArgs e)
        {
            var contr = sender as Control;
            ActionsPerControl a;
            if (contr == null || !_actionPerControl.TryGetValue(contr, out a)) return;

            a.OnClickAction(e.Button).Wait();
        }
        private void Cont_MouseDoubleClickAction(object sender, MouseEventArgs e)
        {
            var contr = sender as Control;
            ActionsPerControl a;
            if (contr == null || !_actionPerControl.TryGetValue(contr, out a)) return;
            a.OnDoubleClickAction(e.Button).Wait();
        }


        private ActionsPerControl getOrCreateActionContainer(Control cont)
        {
            ActionsPerControl o = null;

            if (!_actionPerControl.TryGetValue(cont, out o))
            {
                o = new ActionsPerControl();
                _actionPerControl.Add(cont, o);
                cont.MouseDoubleClick += Cont_MouseDoubleClickAction;
                cont.MouseClick += Cont_MouseClickAction;
            }

            return o;
        } 
        #endregion

        public void Bind(ICommandWrapper cmd, Control clickable)
        {
            var bi = new CommandToClickCommandEnabledUpdater(cmd, clickable);
            this.bindings.Add(bi);

            // TODO implemen can execute changed in the same manner
            clickable.Click += async (object sender, EventArgs e) =>
            {

                if (cmd.CanExecute(null))
                    await cmd.Execute(null);

            };
        }
        public void Bind(ICommandWrapper cmd, ToolStripItem clickable)
        {
            var bi = new CommandToControlBindingItem(cmd, clickable);
            this.bindings.Add(bi);
            clickable.Enabled = cmd.CanExecute(null);
            cmd.CanExecuteChanged += (object sender, EventArgs e) =>
            {
                clickable.Enabled = cmd.CanExecute(null);
            };


            clickable.Click += (object sender, EventArgs e) =>
            {

                if (cmd.CanExecute(null))
                    cmd.Execute(null);

            };
        }


        class CommandToKeyBindingItem : IBindingItem
        {
            private Keys[] keys;
            
            public CommandToKeyBindingItem(ICommandWrapper c, params Keys[] k)
            {
                if (c == null ) throw new ArgumentNullException();
                keys = k;
                Command = c;
            }          
            private ICommandWrapper Command
            {
                get; set;
            }
            public async Task Exec()
            {
               await Command.Execute(null);
            }

            public bool CanCallCommand()
            {

                foreach (var k in keys)
                {
                    if (!KeyMessageFilter.Instance.IsKeyPressed(k))
                        return false;
                }
                
                return Command.CanExecute(null);
            }
            public void OnCanExecuteChanged()
            {
                Command.OnCanExecuteChanged();
            }
        }

        class CommandToControlBindingItem : IBindingItem
        {
            private Control control;



            public CommandToControlBindingItem(ICommandWrapper cmd, Control c)
            {
                Command = cmd;
                control = c;

            }

            private ToolStripItem tsi;
            public CommandToControlBindingItem(ICommandWrapper cmd, ToolStripItem c)
            {
                Command = cmd;
                tsi = c;

            }

            private ICommandWrapper Command
            {
                get;
                set;
            }


            public async Task Exec()
            {
                // intentionally empty

            }

            public bool CanCallCommand()
            {
                var res = Command.CanExecute(null);
                return res;
            }
            public void OnCanExecuteChanged()
            {
                if(Command!=null)
                Command.OnCanExecuteChanged();
            }
        }



        class CommandToClickCommandEnabledUpdater : IBindingItem
        {
            public CommandToClickCommandEnabledUpdater(ICommandWrapper cmd, Control c)
            {
                Command = cmd;
                Control = c;
            }

            private ICommandWrapper Command { get; set; }
            private Control Control { get; set; }
            public bool CanCallCommand()
            {

                var res = Command.CanExecute(null);
                Control.Enabled = res;
                return res;
            }

            public async Task Exec()
            {
                //await Command.Execute(null);

            }

            public void OnCanExecuteChanged()
            {
                if(Command!=null)
                Command.OnCanExecuteChanged();
            }
        }
        interface IBindingItem
        {
            bool CanCallCommand();
            Task Exec();
            void OnCanExecuteChanged();
            // ICommandWrapper Command { get; }
        }

        class BindingItemWithLastValue
        {
            public bool LastCanCallResult { get; set; }
            public IBindingItem BingingItem { get; set; }
            public BindingItemWithLastValue(IBindingItem item)
            {
                this.BingingItem = item;
            }
            public BindingItemWithLastValue(IBindingItem item, bool value)
            {
                this.BingingItem = item;
                LastCanCallResult = value;
            }
        }
        class BindingList : IEnumerable<BindingItemWithLastValue>, IDisposable
        {

            private IList<BindingItemWithLastValue> bindings = new List<BindingItemWithLastValue>();
            public void Add(IBindingItem item)
            {
                bindings.Add(new BindingItemWithLastValue(item, item.CanCallCommand()));
            }

            public IEnumerator<BindingItemWithLastValue> GetEnumerator()
            {
                return bindings.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return bindings.GetEnumerator();
            }

            public void Dispose()
            {
                // todo dispose properly
            }
        }
    }
}

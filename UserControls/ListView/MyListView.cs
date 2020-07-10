using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class MyListView : ListView
{   //https://stackoverflow.com/questions/2691726/how-can-i-remove-the-selection-border-on-a-listviewitem

    // Variables                                                                                                                    
    private bool _firstTime;


    // Constructors                                                                                                                 
    public MyListView()
    {
        // Use first time to disable OnSelectedIndexChanged when the control is Shown for the first time
        _firstTime = true;
    }


    // Overrides                                                                                                                    
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (_firstTime) _firstTime = false;
        //
        base.OnMouseDown(e);
    }
    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        if (_firstTime) _firstTime = false;
        else
        {
            base.OnSelectedIndexChanged(e);
            Message m = Message.Create(this.Handle, 0x127, new IntPtr(0x10001), new IntPtr(0));
            this.WndProc(ref m);
        }
    }
    protected override void OnEnter(EventArgs e)
    {
        base.OnEnter(e);
        Message m = Message.Create(this.Handle, 0x127, new IntPtr(0x10001), new IntPtr(0));
        this.WndProc(ref m);
    }

    
}
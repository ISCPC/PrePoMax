using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace UserControls
{
	/// <summary>
	/// Class extending the <see cref="ColorEditor"/> which adds the
	/// capability to also change the alpha value of the color.
	/// </summary>
	public class ColorEditorEx : ColorEditor
	{
		#region Class ColorUIWrapper

		/// <summary>
		/// Wrapper for the private ColorUI class nested within <see cref="ColorEditor"/>.
		/// It publishes its internals via reflection and adds a <see cref="TrackBar"/> to
		/// adjust teh alpha value.
		/// </summary>
		public class ColorUIWrapper
		{
			#region Fields

			private Control _control;
			private MethodInfo _startMethodInfo;
			private MethodInfo _endMethodInfo;
			private PropertyInfo _valuePropertyInfo;
			private TrackBar _tbAlpha;
			private Label _lblAlpha;
			private bool _inSizeChange = false;

			#endregion

			#region Constructors

			/// <summary>
			/// Creates a new instance.
			/// </summary>
			/// <param name="colorEditor">The editor this instance belongs to.</param>
			public ColorUIWrapper(ColorEditorEx colorEditor) 
			{
				Type colorUiType = typeof(ColorEditor).GetNestedType("ColorUI", BindingFlags.CreateInstance | BindingFlags.NonPublic);
				ConstructorInfo constructorInfo = colorUiType.GetConstructor(new Type[] { typeof(ColorEditor) });
				_control = (Control)constructorInfo.Invoke(new object[] { colorEditor });

				Panel alphaPanel = new Panel();
				alphaPanel.BackColor = SystemColors.Control;
				alphaPanel.Dock = DockStyle.Right;
				alphaPanel.Width = 28;
				_control.Controls.Add(alphaPanel);
				_tbAlpha = new TrackBar();
				_tbAlpha.Orientation = Orientation.Vertical;
				_tbAlpha.Dock = DockStyle.Fill;
				_tbAlpha.TickStyle = TickStyle.None;
				_tbAlpha.Maximum = byte.MaxValue;
				_tbAlpha.Minimum = byte.MinValue;
				_tbAlpha.ValueChanged += new EventHandler(OnTrackBarAlphaValueChanged);
				alphaPanel.Controls.Add(_tbAlpha);
				_lblAlpha = new Label();
				_lblAlpha.Text = "0";
				_lblAlpha.Dock = DockStyle.Bottom;
				_lblAlpha.TextAlign = ContentAlignment.MiddleCenter;
				alphaPanel.Controls.Add(_lblAlpha);

				_startMethodInfo = _control.GetType().GetMethod("Start");
				_endMethodInfo = _control.GetType().GetMethod("End");
				_valuePropertyInfo = _control.GetType().GetProperty("Value");

				_control.SizeChanged += new EventHandler(OnControlSizeChanged);
			}

			#endregion

			#region Public interface

			/// <summary>
			/// The control to be shown when a color is edited.
			/// The concrete type is ColorUI which is privately hidden
			/// within System.Drawing.Design.
			/// </summary>
			public Control Control 
			{
				get { return _control; }
			}

			/// <summary>
			/// Gets the edited color with applied alpha value.
			/// </summary>
			public object Value 
			{
				get 
				{
					object result = _valuePropertyInfo.GetValue(_control, new object[0]);
					if (result is Color)
						result = Color.FromArgb(_tbAlpha.Value, (Color)result);
					return result;
				}
			}

			/// <summary>
			/// Starts the editing process.
			/// </summary>
			/// <param name="service">The editor service.</param>
			/// <param name="value">The value to be edited.</param>
			public void Start(IWindowsFormsEditorService service, object value) 
			{
				if (value is Color)
					_tbAlpha.Value = ((Color)value).A;

				_startMethodInfo.Invoke(_control, new object[] { service, value });
			}

			/// <summary>
			/// End the editing process.
			/// </summary>
			public void End() 
			{
				_endMethodInfo.Invoke(_control, new object[0]);
			}

			#endregion

			#region Privates

			private void OnControlSizeChanged(object sender, EventArgs e)
			{
				if (_inSizeChange)
					return;

				try 
				{
					_inSizeChange = true;

					TabControl tabControl = (TabControl)_control.Controls[0];

					Size size = tabControl.TabPages[0].Controls[0].Size;
					Rectangle rectangle = tabControl.GetTabRect(0);
					_control.Size = new Size(_tbAlpha.Width + size.Width, size.Height + rectangle.Height);
				} 
				finally 
				{
					_inSizeChange = false;
				}
			}

			#endregion

			private void OnTrackBarAlphaValueChanged(object sender, EventArgs e)
			{
				_lblAlpha.Text = _tbAlpha.Value.ToString();
			}
		}

		#endregion

		#region Fields

		private ColorUIWrapper _colorUI;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ColorEditorEx() { }

		#endregion

		#region Overridden from ColorEditor

		/// <summary>
		/// Edits the given value.
		/// </summary>
		/// <param name="context">Context infromation.</param>
		/// <param name="provider">Service provider.</param>
		/// <param name="value">Value to be edited.</param>
		/// <returns>An edited value.</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
				if (service == null)
					return value;

				if (_colorUI == null)
					_colorUI = new ColorUIWrapper(this);

				_colorUI.Start(service, value);
				service.DropDownControl(_colorUI.Control);
				if ((_colorUI.Value != null) && (((Color) _colorUI.Value) != Color.Empty))
				{
					value = _colorUI.Value;
				}
				_colorUI.End();
			}
			return value;
		}

        public override void PaintValue(PaintValueEventArgs e)
{
	if (e.Value is Color && ((Color)e.Value).A < byte.MaxValue) 
	{
		int oneThird = e.Bounds.Width / 3;
		using (SolidBrush brush = new SolidBrush(Color.White))
		{
			e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.X, e.Bounds.Y, oneThird, e.Bounds.Height - 1));
		}
		using (SolidBrush brush = new SolidBrush(Color.DarkGray))
		{
			e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.X + oneThird, e.Bounds.Y, oneThird, e.Bounds.Height - 1));
		}
		using (SolidBrush brush = new SolidBrush(Color.Black))
		{
			e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.X + oneThird * 2, e.Bounds.Y, e.Bounds.Width - oneThird * 2, e.Bounds.Height - 1));
		}
	}

	base.PaintValue(e);
}

		#endregion
	}
}

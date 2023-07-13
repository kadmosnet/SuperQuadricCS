using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXFReaderNET;
using DXFReaderNET.Header;
using Microsoft.Win32;
namespace SuperQuadricCS
{
    public partial class Form1 : Form
    {

        private bool OnRotate = false;
        private bool mousedown = false;
       
        private Vector2 orbitPointStart = Vector2.Zero;
        private readonly char decimalSeparetor = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray()[0];
        public Form1()
        {
            InitializeComponent();
        }

        private double rxg, ryg, rzg, a1, a2, a3, eps1, eps2, eta, omega;
        private int num, r, p, j, k, v1, v2, va1, va2;
        private short color;

        private void Button14_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.ZoomExtents();
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            dxfReaderNETControl1.Rendering = RenderingType.WireFrame;
            dxfReaderNETControl1.Refresh();
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            dxfReaderNETControl1.Rendering = RenderingType.Shaded;
            dxfReaderNETControl1.Refresh();
        }

        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            dxfReaderNETControl1.Rendering = RenderingType.ShadedEdges;
            dxfReaderNETControl1.Refresh();
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            {

                SaveFileDialog1.DefaultExt = "jpg";
                SaveFileDialog1.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp";




                SaveFileDialog1.FilterIndex = 1;
                if (SaveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    switch (SaveFileDialog1.FilterIndex)
                    {
                        case 1:
                            {
                                dxfReaderNETControl1.Image.Save(SaveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;
                            }

                        case 2:
                            {
                                dxfReaderNETControl1.Image.Save(SaveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                                break;
                            }

                        case 3:
                            {
                                dxfReaderNETControl1.Image.Save(SaveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                break;
                            }
                    }
                }
            }
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            {

                dxfReaderNETControl1.NewDrawing();
                color = 1;
                r = 15;
                p = 30;
                rxg = 0;
                ryg = 0;
                rzg = 0;
                eps1 = 1;
                eps2 = 1;
                Center.X = 0;
                Center.Y = 0;
                Center.Z = 0;

                a1 = 20;
                a2 = 20;
                a3 = 20;

                int k;
                int j;
                color = 10;
                eps1 = 0.5;
                eps2 = 0.5;

                for (j = 0; j <= 3; j++)
                {
                    for (k = 0; k <= 5; k++)
                    {
                        Center.Y = j * 60;
                        Center.X = k * 60;
                        GenerateSuperQuadric();
                        color += 10;
                        eps1 += 0.05;
                        eps2 += 0.05;
                    }
                }

                DisplayView(PredefinedViewType.SW_Isometric);

            }
        }

        private Vector3 Center;

        private void Button1_Click(object sender, EventArgs e)
        {
            r = Convert.ToInt32(TextBoxRings.Text);
            p = Convert.ToInt32(TextBoxPointsXring.Text);
            rxg = Convert.ToDouble(TextBoxRotationX.Text);
            ryg = Convert.ToDouble(TextBoxRotationY.Text);
            rzg = Convert.ToDouble(TextBoxRotationZ.Text);
            eps1 = Convert.ToDouble(TextBoxEps1.Text);
            eps2 = Convert.ToDouble(TextBoxEps2.Text);
            Center.X = Convert.ToDouble(TextBoxCenterX.Text);
            Center.Y = Convert.ToDouble(TextBoxCenterY.Text);
            Center.Z = Convert.ToDouble(TextBoxCenterZ.Text);

            a1 = Convert.ToDouble(TextBoxRadiusX.Text);
            a2 = Convert.ToDouble(TextBoxRadiusY.Text);
            a3 = Convert.ToDouble(TextBoxRadiusZ.Text);
           
            color = Convert.ToInt16(TextBoxColor.Text);
            GenerateSuperQuadric();
           
            dxfReaderNETControl1.DXF.VPorts["*Active"].ViewDirection = new Vector3(-Math.Sqrt(1 / (double)3), -Math.Sqrt(1 / (double)3), Math.Sqrt(1 / (double)3));
            dxfReaderNETControl1.Refresh();
            dxfReaderNETControl1.ZoomExtents();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveRegistry();
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            TextBoxColor.Text = dxfReaderNETControl1.ShowPalette(AciColor.FromCadIndex(Convert.ToInt16(TextBoxColor.Text))).ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dxfReaderNETControl1.NewDrawing();
            dxfReaderNETControl1.Rendering = RenderingType.Shaded;
            LoadRegistry();
        }

        private void TextBoxCenterX_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);

        }
        private void ForceNumeric(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != decimalSeparetor) && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == decimalSeparetor) && ((sender as TextBox).Text.IndexOf(decimalSeparetor) > -1))
            {
                e.Handled = true;
            }
        }

        private void TextBoxRotationX_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }

        private void TextBoxRotationY_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }

        private void TextBoxRotationZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }

        private void TextBoxCenterY_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }

        private void TextBoxCenterZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }

        private void TextBoxRadiusX_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }

        private void TextBoxRadiusY_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }

        private void TextBoxRadiusZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }

        private void TextBoxEps1_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }

        private void TextBoxEps2_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForceNumeric(sender, e);
        }
        private void DisplayView(PredefinedViewType viewType)
        {
            dxfReaderNETControl1.DisplayPredefinedView(viewType);
            dxfReaderNETControl1.Refresh();
            dxfReaderNETControl1.ZoomExtents();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            OnRotate = true;
        }

        private void dxfReaderNETControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!mousedown && OnRotate)
            {
                mousedown = true;
                
                dxfReaderNETControl1.CustomCursor = CustomCursorType.None;
                orbitPointStart = dxfReaderNETControl1.CurrentWCSpoint;
            }
        }

        private void dxfReaderNETControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown && OnRotate)
            {
               

                dxfReaderNETControl1.Orbit(dxfReaderNETControl1.CurrentWCSpoint, orbitPointStart);
            }
        }

        private void dxfReaderNETControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (OnRotate)
            {
                mousedown = false;
                OnRotate = false;

            }
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.About();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            DisplayView(PredefinedViewType.Top);

        }

        private void Button5_Click(object sender, EventArgs e)
        {
            DisplayView(PredefinedViewType.Bottom);

        }

        private void Button6_Click(object sender, EventArgs e)
        {
            DisplayView(PredefinedViewType.Left);

        }

        private void Button7_Click(object sender, EventArgs e)
        {
            DisplayView(PredefinedViewType.Right);

        }

        private void Button8_Click(object sender, EventArgs e)
        {
            DisplayView(PredefinedViewType.Front);

        }

        private void Button9_Click(object sender, EventArgs e)
        {
            DisplayView(PredefinedViewType.Back);

        }

        private void Button10_Click(object sender, EventArgs e)
        {

            DisplayView(PredefinedViewType.SW_Isometric);

        }

        private void Button11_Click(object sender, EventArgs e)
        {
            DisplayView(PredefinedViewType.SE_Isometric);

        }

        private void Button12_Click(object sender, EventArgs e)
        {
            DisplayView(PredefinedViewType.NE_Isometric);

        }

        private void Button13_Click(object sender, EventArgs e)
        {
            DisplayView(PredefinedViewType.NW_Isometric);

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.NewDrawing();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            {

                SaveFileDialog1.DefaultExt = "dxf";
                SaveFileDialog1.Filter = "AutoCAD R10 DXF|*.dxf"; // 1
                SaveFileDialog1.Filter += "|AutoCAD R11 and R12 DXF|*.dxf"; // 2
                SaveFileDialog1.Filter += "|AutoCAD R13 DXF|*.dxf"; // 3
                SaveFileDialog1.Filter += "|AutoCAD R14 DXF|*.dxf"; // 4
                SaveFileDialog1.Filter += "|AutoCAD 2000 DXF|*.dxf"; // 5
                SaveFileDialog1.Filter += "|AutoCAD 2004 DXF|*.dxf"; // 6
                SaveFileDialog1.Filter += "|AutoCAD 2007 DXF|*.dxf"; // 7
                SaveFileDialog1.Filter += "|AutoCAD 2010 DXF|*.dxf"; // 8
                SaveFileDialog1.Filter += "|AutoCAD 2013 DXF|*.dxf"; // 9
                SaveFileDialog1.Filter += "|AutoCAD 2018 DXF|*.dxf"; // 10

                SaveFileDialog1.Filter += "|AutoCAD R10 binary DXF|*.dxf"; // 11
                SaveFileDialog1.Filter += "|AutoCAD R11 and R12 binary DXF|*.dxf"; // 12
                SaveFileDialog1.Filter += "|AutoCAD R13 binary DXF|*.dxf"; // 13
                SaveFileDialog1.Filter += "|AutoCAD R14 binary DXF|*.dxf"; // 14
                SaveFileDialog1.Filter += "|AutoCAD 2000 binary DXF|*.dxf"; // 15
                SaveFileDialog1.Filter += "|AutoCAD 2004 binary DXF|*.dxf"; // 16
                SaveFileDialog1.Filter += "|AutoCAD 2007 binary DXF|*.dxf"; // 17
                SaveFileDialog1.Filter += "|AutoCAD 2010 binary DXF|*.dxf"; // 18
                SaveFileDialog1.Filter += "|AutoCAD 2013 binary DXF|*.dxf"; // 19
                SaveFileDialog1.Filter += "|AutoCAD 2018 binary DXF|*.dxf"; // 20


                switch (dxfReaderNETControl1.DXF.DrawingVariables.AcadVer)
                {
                    case DxfVersion.AutoCad10:
                        {
                            SaveFileDialog1.FilterIndex = 1;
                            break;
                        }

                    case DxfVersion.AutoCad12:
                        {
                            SaveFileDialog1.FilterIndex = 2;
                            break;
                        }

                    case DxfVersion.AutoCad13:
                        {
                            SaveFileDialog1.FilterIndex = 3;
                            break;
                        }

                    case DxfVersion.AutoCad14:
                        {
                            SaveFileDialog1.FilterIndex = 4;
                            break;
                        }

                    case DxfVersion.AutoCad2000:
                        {
                            SaveFileDialog1.FilterIndex = 5;
                            break;
                        }

                    case DxfVersion.AutoCad2004:
                        {
                            SaveFileDialog1.FilterIndex = 6;
                            break;
                        }

                    case DxfVersion.AutoCad2007:
                        {
                            SaveFileDialog1.FilterIndex = 7;
                            break;
                        }

                    case DxfVersion.AutoCad2010:
                        {
                            SaveFileDialog1.FilterIndex = 8;
                            break;
                        }

                    case DxfVersion.AutoCad2013:
                        {
                            SaveFileDialog1.FilterIndex = 9;
                            break;
                        }

                    case DxfVersion.AutoCad2018:
                        {
                            SaveFileDialog1.FilterIndex = 10;
                            break;
                        }
                }

                if (dxfReaderNETControl1.FileName != null)
                {
                    System.IO.FileInfo FileInfo = new System.IO.FileInfo(dxfReaderNETControl1.FileName);

                    SaveFileDialog1.FileName = FileInfo.Name;
                }
                else
                    SaveFileDialog1.FileName = "SuperQuadric.dxf";


                if (dxfReaderNETControl1.DXF.IsBinary)
                    SaveFileDialog1.FilterIndex += 10;

                if (SaveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    DxfVersion dxfver = DxfVersion.AutoCad2013;
                    switch (SaveFileDialog1.FilterIndex)
                    {
                        case 1:
                        case 11:
                            {
                                dxfver = DxfVersion.AutoCad10;
                                break;
                            }

                        case 2:
                        case 12:
                            {
                                dxfver = DxfVersion.AutoCad12;
                                break;
                            }

                        case 3:
                        case 13:
                            {
                                dxfver = DxfVersion.AutoCad13;
                                break;
                            }

                        case 4:
                        case 14:
                            {
                                dxfver = DxfVersion.AutoCad14;
                                break;
                            }

                        case 5:
                        case 15:
                            {
                                dxfver = DxfVersion.AutoCad2000;
                                break;
                            }

                        case 6:
                        case 16:
                            {
                                dxfver = DxfVersion.AutoCad2004;
                                break;
                            }

                        case 7:
                        case 17:
                            {
                                dxfver = DxfVersion.AutoCad2007;
                                break;
                            }

                        case 8:
                        case 18:
                            {
                                dxfver = DxfVersion.AutoCad2010;
                                break;
                            }

                        case 9:
                        case 19:
                            {
                                dxfver = DxfVersion.AutoCad2013;
                                break;
                            }

                        case 10:
                        case 20:
                            {
                                dxfver = DxfVersion.AutoCad2018;
                                break;
                            }
                    }

                    if (SaveFileDialog1.FilterIndex > 10)
                        dxfReaderNETControl1.DXF.IsBinary = true;

                    dxfReaderNETControl1.WriteDXF(SaveFileDialog1.FileName, dxfver, dxfReaderNETControl1.DXF.IsBinary);
                }
            }
        }


        private void GenerateSuperQuadric()
        {
            double alpha = rxg * Math.PI / 180;
            double beta = ryg * Math.PI / 180;
            double gamma = rzg * Math.PI / 180;

            Vector3[] matr = null;

            num = 0;
            eta = Math.PI / 2;
            for (j = 1; j <= r; j++)
            {
                omega = 0;
                for (k = 1; k <= p; k++)
                {
                    var oldMatr = matr;
                    matr = new Vector3[num + 1 + 1];
                    if (oldMatr != null)
                        Array.Copy(oldMatr, matr, Math.Min(num + 1 + 1, oldMatr.Length));
                    matr[num + 1] = Superellipsoid(eta, omega);
                    num += 1;
                    omega += 2 * Math.PI / p;
                }
                eta -= Math.PI / (r - 1);
            }

            //Matrix m = new Matrix();
            for (k = 1; k <= matr.Length - 1; k++)
            {
                matr[k] = Matrix.RotationX(alpha) * Matrix.RotationX(beta) * Matrix.RotationX(gamma) * matr[k];

                matr[k] += Center;
            }

            // dxf generation
            for (k = 0; k <= r * p - p - 1; k++)
            {
                v1 = k + p + 1;
                v2 = k + 1;
                va1 = v1;
                va2 = v2;


                if (v1 % p == 0)
                {
                    va1 = v1 - p;
                    va2 = v2 - p;
                }
                Vector3 FirstVertex;
                Vector3 SecondVertex;
                Vector3 ThirdVertex;
                Vector3 FourthVertex;

                FirstVertex = matr[k + 1];
                SecondVertex = matr[k + p + 1];
                ThirdVertex = matr[va1 + 1];
                FourthVertex = matr[va2 + 1];

                dxfReaderNETControl1.AddFace3D(FirstVertex, SecondVertex, ThirdVertex, FourthVertex, color);
            }
        }

        private Vector3 Superellipsoid(double eta, double omega)
        {
            Vector3 v = Vector3.Zero;

            v.X = a1 * elev(Math.Cos(eta), eps1) * elev(Math.Cos(omega), eps2);
            v.Y = a2 * elev(Math.Cos(eta), eps1) * elev(Math.Sin(omega), eps2);
            v.Z = a3 * elev(Math.Sin(eta), eps1);
            return v;
        }

       

        private double elev(double x, double y)
        {
            double buf = 0;
            double e = 0;
            bool test = true;

            if (x != 0)
                buf = y * Math.Log(Math.Abs(x));
            else
                test = false;
            if (buf > -88)
                e = Math.Exp(buf) * Math.Sign(x);
            else
                test = false;
            if (!test)
                e = 0;
            return e;
        }


        private void LoadRegistry()
        {
            Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("SuperQuadricCS");
            this.Width = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\SuperQuadricCS", "m_wWidth", Screen.PrimaryScreen.Bounds.Width - 40);
            this.Height = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\SuperQuadricCS", "m_wHeight", Screen.PrimaryScreen.Bounds.Height - 60);
            this.Left = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\SuperQuadricCS", "m_wLeft", 20);
            this.Top = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\SuperQuadricCS", "m_wTop", 20);
        }

        private void SaveRegistry()
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\SuperQuadricCS", "m_wWidth", this.Width);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\SuperQuadricCS", "m_wHeight", this.Height);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\SuperQuadricCS", "m_wLeft", this.Left);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\SuperQuadricCS", "m_wTop", this.Top);
        }


    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Windows.Forms;
using Coding4Fun.Kinect.Wpf;
using System.Runtime.InteropServices;
using System.Threading;
using Fizbin.Kinect.Gestures;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using Microsoft.Samples.Kinect.WpfViewers;
using Microsoft.Kinect.Toolkit;
namespace kinectapp
    
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
           

        public MainWindow()
        {
            InitializeComponent();
        }
         //  [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]

    

        [DllImport("user32.dll")]
        
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        


        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }


          // private const int MOUSEEVENTF_LEFTDOWN = 0x02;
           //private const int MOUSEEVENTF_LEFTUP = 0x04;
           //private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
           //private const int MOUSEEVENTF_RIGHTUP = 0x10;

        KinectSensorChooser kinectSensorChooser = new KinectSensorChooser();

        private System.Timers.Timer timer1 = new System.Timers.Timer(500);
        const int skeletonCount= 6;
        Skeleton[] allSkeletons= new Skeleton[skeletonCount];
        KinectSensor newSensor;
        private GestureController gestureController;
        int mode = 0;
        int flag = 0;
        int flag3 = 0;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.kinectSensorChooserUI1.KinectSensorChooser = kinectSensorChooser;

            kinectSensorChooser.KinectChanged += new EventHandler<KinectChangedEventArgs>(kinectSensorChooser_KinectChanged);
            kinectSensorChooser.Start();
            textBox1.Text = "blah";
        }

        void kinectSensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
            KinectSensor oldSensor = (KinectSensor)e.OldSensor;
            if(oldSensor!=null)oldSensor.Stop();
            
            KinectSensor newSensor = (KinectSensor)e.NewSensor;

            if (newSensor == null)
            {
                return;
            }

            //register for event and enable Kinect sensor features you want
            newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(newSensor_AllFramesReady);
            newSensor.ColorStream.Enable();
            newSensor.DepthStream.Enable();
            newSensor.SkeletonStream.Enable();

            try
            {
                newSensor.Start();
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser.TryResolveConflict();

            }

            // initialize the gesture recognizer
            gestureController = new GestureController();
            gestureController.GestureRecognized += new EventHandler<GestureEventArgs>(gestureController_GestureRecognized);

        }
        private string _gesture;
        public String Gesture
        {
            get { return _gesture; }

            private set
            {
                if (_gesture == value)
                {
                    return;
                }
                _gesture = value;

                if (mode == 0)
                {
                    if (_gesture == "Swipe Right")
                    {
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        keybd_event(0x27, 0, 0x0, 0);
                        // keybd_event(0x26, 0, 0x0, 0);
                        timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
                        timer1.Interval = 1000;      //time value to be tweaked according to testing
                        timer1.Enabled = true;
                        //keybd_event(0x14, 0, 0x2, 0);
                    }

                    if (_gesture == "Swipe Left")
                    {
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        keybd_event(0x25, 0, 0x0, 0);
                        // keybd_event(0x26, 0, 0x0, 0);
                        timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
                        timer1.Interval = 1000;      //time value to be tweaked according to testing
                        timer1.Enabled = true;
                        //keybd_event(0x14, 0, 0x2, 0);
                    }
                    if (_gesture == "Zoom In")
                    {
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        keybd_event(0x11, 0, 0x0, 0);       // ctrl key
                        keybd_event(0x26, 0, 0x0, 0);       // up arrow key
                        timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
                        timer1.Interval = 1000;                 // time to be changed according to settings
                        timer1.Enabled = true;
                        //keybd_event(0x14, 0, 0x2, 0);
                    }
                    if (_gesture == "Zoom Out")
                    {
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        keybd_event(0x11, 0, 0x0, 0);       // ctrl key
                        keybd_event(0x28, 0, 0x0, 0);       // down arrow key
                        timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
                        timer1.Interval = 1000;                 // time to be changed according to settings
                        timer1.Enabled = true;
                        //keybd_event(0x14, 0, 0x2, 0);
                    }
                }

                if (mode == 1)
                {



                    if (_gesture == "Swipe Right")
                    {
                        keybd_event(0x4C, 0, 0x0, 0);           //l key
                        keybd_event(0x4C, 0, 0x2, 0);
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        _gesture = null;
                    }

                    if (_gesture == "Swipe Left")
                    {
                        keybd_event(0x4A, 0, 0x0, 0);           //j key
                        keybd_event(0x4A, 0, 0x2, 0);
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        _gesture = null;
                    }
                    if (_gesture == "Zoom In")
                    {
                        keybd_event(0xBB, 0, 0x0, 0);       //+ key
                        keybd_event(0xBB, 0, 0x2, 0);
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        _gesture = null;
                    }

                    if (_gesture == "Zoom Out")
                    {
                        keybd_event(0xBD, 0, 0x0, 0);           //+ key
                        keybd_event(0xBD, 0, 0x2, 0);
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        _gesture = null;
                    }
                    /*
                    if (_gesture == "Joined Hands")
                    {
                        keybd_event(0x4B, 0, 0x0, 0);           //k key
                        keybd_event(0x4B, 0, 0x2, 0);
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        _gesture = null;
                    }
                    */

                }

                if (mode == 2)
                {
                    if (_gesture == "Swipe Right")
                    {
                        keybd_event(0x56, 0, 0x0, 0);               //v key
                        keybd_event(0x56, 0, 0x2, 0);
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        _gesture = null;
                    }

                    if (_gesture == "Swipe Left")
                    {
                        keybd_event(0x43, 0, 0x0, 0);           //C key
                        keybd_event(0x43, 0, 0x2, 0);
                        textBox2.Text = "mode = " + mode + "    gesture =  " + _gesture;
                        _gesture = null;
                    }
                }
                //Debug.WriteLine("Gesture = " + _gesture);
                // textBox2.Text = _gesture + mode;

                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
            }


        }

    /*   void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           // throw new NotImplementedException();
          //oldSensor = (KinectSensor)e.OldValue;
           // oldSensor.Stop();
           // oldSensor.AudioSource.Stop();

            newSensor = (KinectSensor)e.NewValue;
            newSensor.ColorStream.Enable();
            newSensor.DepthStream.Enable();
            var param = new TransformSmoothParameters
            {
                Smoothing = .1f,
                Correction = 0.0f,
                Prediction = .3f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = .5f
            };
            newSensor.SkeletonStream.Enable(param);
            newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(newSensor_AllFramesReady);

            try
            {
                newSensor.Start();
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
                //throw;
            }
         
        }*/

        void timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            keybd_event(0x27, 0, 0x2, 0);
            keybd_event(0x25, 0, 0x2, 0);
            keybd_event(0x26, 0, 0x2, 0);
            keybd_event(0x28, 0, 0x2, 0);
            keybd_event(0x11, 0, 0x2, 0);       // ctrl key
            timer1.Enabled = false;
            _gesture = null;
            flag = 0;
        }


        public event PropertyChangedEventHandler PropertyChanged;


        void gestureController_GestureRecognized(object sender, GestureEventArgs e)
        {
            Debug.WriteLine(e.GestureType);

            switch (e.GestureType)
            {
                case GestureType.Menu:
                    Gesture = "Menu";
                    break;
                case GestureType.WaveRight:
                    Gesture = "Wave Right";
                    break;
                case GestureType.WaveLeft:
                    Gesture = "Wave Left";
                    break;
                case GestureType.JoinedHands:
                    Gesture = "Joined Hands";
                    break;
                case GestureType.SwipeLeft:
                    Gesture = "Swipe Left";
                    break;
                case GestureType.SwipeRight:
                    Gesture = "Swipe Right";
                    break;
                case GestureType.ZoomIn:
                    Gesture = "Zoom In";
                    break;
                case GestureType.ZoomOut:
                    Gesture = "Zoom Out";
                    break;

                default: Gesture = "None";
                    break;
            }
        }

        Skeleton getSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }
                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                Skeleton first = (from s in allSkeletons where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                return first;
            }
        }
        int flag2=0;
        void GetCameraPoint ( Skeleton first , AllFramesReadyEventArgs e)
        {
            using( DepthImageFrame depth= e.OpenDepthImageFrame())
            {
                if(depth==null || kinectSensorChooser.Kinect==null)
                {
                    return;
                }
                DepthImagePoint handDepthPoint= depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);
                //public static Cursor Current { get;}
                 //Cursor.Position = new Point(handDepthPoint.X, handDepthPoint.Y);
                System.Windows.Forms.Control control = new System.Windows.Forms.Control();
                Joint scaledJoint = first.Joints[JointType.HandRight];
                Joint scaledJointleft = first.Joints[JointType.HandLeft];
                Joint scaledJointNeck = first.Joints[JointType.ShoulderCenter];
                Joint KneeRight = first.Joints[JointType.KneeRight]; 
                if (scaledJoint.Position.Y - scaledJointNeck.Position.Y > 0 && mode == 0 && flag == 0)
                {
                    keybd_event(0x26, 0, 0x0, 0);
                    timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
                    timer1.Interval = 1000;      //time value to be tweaked according to testing
                    timer1.Enabled = true;
                    flag = 1;
                }

                if (scaledJoint.Position.Y - scaledJointNeck.Position.Y > 0 && mode == 1 && flag == 0)
                {
                    keybd_event(0x4B, 0, 0x0, 0);           //k key
                    keybd_event(0x4B, 0, 0x2, 0);
                    flag = 1;
                }

                if (scaledJoint.Position.Y - scaledJointNeck.Position.Y > 0 && mode == 2 && flag3 == 0)
                {
                    keybd_event(0x52, 0, 0x0, 0);           //r key
                    keybd_event(0x52, 0, 0x2, 0);
                    flag3 = 1;
                }

                if (KneeRight.Position.Y - scaledJoint.Position.Y > 0 && mode == 0 && flag2 == 0)
                {
                    keybd_event(0x28, 0, 0x0, 0);
                    timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
                    timer1.Interval = 1000;      //time value to be tweaked according to testing
                    timer1.Enabled = true;
                    flag2 = 1;
                }



                if (KneeRight.Position.Y - scaledJoint.Position.Y > 0 && mode == 1 && flag2 == 0)
                {
                    keybd_event(0x38, 0, 0x0, 0);           //key 8
                    keybd_event(0x38, 0, 0x2, 0);
                    flag2 = 1;
                }

                if (KneeRight.Position.Y - scaledJoint.Position.Y < 0)
                {
                   
                    flag2 = 0;
                }

                if (scaledJointleft.Position.Y - scaledJointNeck.Position.Y > 0 && flag == 0)
                {
                    mode++;
                    if (mode == 3)
                        mode = 0;
                    flag = 1;
                }
                if(scaledJointleft.Position.Y - scaledJointNeck.Position.Y < 0 ){
                    flag = 0;
                }

                if (scaledJoint.Position.Y - scaledJointNeck.Position.Y < 0)
                {
                    flag3 = 0;
                }
               /* if (scaledJointleft.Position.Y - scaledJointNeck.Position.Y > 0 && mode==1 && flag2 ==0)
                {

                    keybd_event(0x20, 0, 0x0, 0);               //space bar key
                    keybd_event(0x20, 0, 0x2, 0);
                    flag2 = 1;
                }*/
              /*  if(scaledJointleft.Position.Y - scaledJointNeck.Position.Y < 0 && mode==1 ){
                    flag2 = 0;
                }*/
                
            //    System.Drawing.Point coordinate = new System.Drawing.Point((int)scaledJoint.Position.X,(int) scaledJoint.Position.Y);
                //System.Windows.Forms.Cursor.Position = control.PointToScreen(coordinate);
               // textBox1.Text = "Outside " + (int)scaledJointNeck.Position.X + " " + (int)scaledJointNeck.Position.X + " " + (int)scaledJoint.Position.Y;
               // if ((((int)scaledJointleft.Position.Y)  - ((int)scaledJointNeck.Position.Y)) >200)
                    //|| ((int)scaledJointNeck.Position.Y - (int)scaledJointleft.Position.Y) > -200) && flag==0)
              //  {
                    
                    //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (int)scaledJointleft.Position.X, (int)scaledJointleft.Position.Y, 0, 0);
                    //System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)scaledJoint.Position.X, (int)scaledJoint.Position.Y);
                    
                    //mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
                    //mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
                  //  textBox1.Text = "Click " + (int)scaledJointNeck.Position.Y + " " + (int)scaledJointNeck.Position.Y + " " + (int)scaledJoint.Position.Y;
                   // keybd_event(0x14, 0, 0x0, 0);
                   
                    //keybd_event(0x14, 0, 0x2, 0);
                  //  flag = 1;
                    
               // }
               // else if ((-((int)scaledJointleft.Position.Y)+((int)scaledJointNeck.Position.Y)) > 0)
               // {
                    
                   // mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
                //    textBox1.Text = "Not Click " + (int)scaledJointNeck.Position.Y + " " + (int)scaledJointNeck.Position.Y + " " + (int)scaledJoint.Position.Y;
                //}
                //if (((int)scaledJointNeck.Position.Y - (int)scaledJointleft.Position.Y) > 200)
                //{
                //    flag = 0;
                //}
                textBox1.Text = scaledJointleft.Position.X + "    " + scaledJoint.Position.X +"        " + "       "+ scaledJointNeck.Position.X +"\n"+
                    scaledJointleft.Position.Y + "    " + scaledJoint.Position.Y + "        " + "       " + scaledJointNeck.Position.Y;

            }
        }



        void newSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {

            Skeleton first = getSkeleton(e);
            if (first == null)
            {
                return;
            }
            GetCameraPoint(first, e);
            gestureController.UpdateAllGestures(first);

           // throw new NotImplementedException();
        }

       
        void Window_Closed(object sender, EventArgs e)
        {
            kinectSensorChooser.Kinect.Stop();
            kinectSensorChooser.Kinect.AudioSource.Stop();
                   }
    }
}

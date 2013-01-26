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
namespace kinectapp
    
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
           

        public MainWindow()
        {
            InitializeComponent();
        }
         //  [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]

    

        [DllImport("user32.dll")]
        
        //static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
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
           int flag=0; 
        
        
        const int skeletonCount= 6;
        Skeleton[] allSkeletons= new Skeleton[skeletonCount];
        KinectSensor newSensor;




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        
        }

        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
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
                Smoothing = .5f,
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

        void GetCameraPoint ( Skeleton first , AllFramesReadyEventArgs e)
        {
            using( DepthImageFrame depth= e.OpenDepthImageFrame())
            {
                if(depth==null || kinectSensorChooser1.Kinect==null)
                {
                    return;
                }
                DepthImagePoint handDepthPoint= depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);
                //public static Cursor Current { get;}
                 //Cursor.Position = new Point(handDepthPoint.X, handDepthPoint.Y);
                System.Windows.Forms.Control control = new System.Windows.Forms.Control();
                
                Joint scaledJoint = first.Joints[JointType.HandRight].ScaleTo(1920, 1080,.2f,.2f);
                Joint scaledJointleft = first.Joints[JointType.HandLeft].ScaleTo(1920, 1080, .2f, .2f);
                Joint scaledJointNeck = first.Joints[JointType.ShoulderCenter].ScaleTo(1920, 1080, .2f, .2f);
                System.Drawing.Point coordinate = new System.Drawing.Point((int)scaledJoint.Position.X,(int) scaledJoint.Position.Y);
                System.Windows.Forms.Cursor.Position = control.PointToScreen(coordinate);
                if ((((int)scaledJointleft.Position.Y)  - ((int)scaledJointNeck.Position.Y)) >100)
                    //|| ((int)scaledJointNeck.Position.Y - (int)scaledJointleft.Position.Y) > -200) && flag==0)
                {
                    
                    //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (int)scaledJointleft.Position.X, (int)scaledJointleft.Position.Y, 0, 0);
                    System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)scaledJoint.Position.X, (int)scaledJoint.Position.Y);
                    
                    //mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
                    mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
                   // keybd_event(0x14, 0, 0x0, 0);
                   
                    //keybd_event(0x14, 0, 0x2, 0);
                    flag = 1;
                    
                }
                else if ((-((int)scaledJointleft.Position.Y) + ((int)scaledJointNeck.Position.Y)) < 0)
                {
                    
                    mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
                }
                //if (((int)scaledJointNeck.Position.Y - (int)scaledJointleft.Position.Y) > 200)
                //{
                //    flag = 0;
                //}
                textBox1.Text = scaledJointleft.Position.X + "    " + scaledJointleft.Position.Y +"        " + "       "+ scaledJointNeck.Position.Y;
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


           // throw new NotImplementedException();
        }

       
        void Window_Closed(object sender, EventArgs e)
        {
            kinectSensorChooser1.Kinect.Stop();
            kinectSensorChooser1.Kinect.AudioSource.Stop();
                   }
    }
}

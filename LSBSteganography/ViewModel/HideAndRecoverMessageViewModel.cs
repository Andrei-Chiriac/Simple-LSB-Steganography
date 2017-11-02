using LSBSteganography.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LSBSteganography.ViewModel
{
    public class HideAndRecoverMessageViewModel : ViewModelBase
    {
        public HideAndRecoverMessageViewModel()
        {
            SelectCarrierImageCommand = new RelayCommand(SelectCarrierImage);
            SelectResultImageCommand = new RelayCommand(SelectResultImage);
            HideMessageCommand = new RelayCommand(HideMessage);
            RecoverMessageCommand = new RelayCommand(RecoverMessage);
        }


        private string _SecretMessage;
        public string SecretMessage
        {
            get => _SecretMessage;
            set
            {
                _SecretMessage = value;
                OnPropertyChanged();
            }
        }

        private string _HidePassword;
        public string HidePassword
        {
            get => _HidePassword;
            set
            {
                _HidePassword = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectCarrierImageCommand { get; set; }


        private ImageSource _CarrierSource;
        public ImageSource CarrierSource
        {
            get => _CarrierSource;
            set
            {
                _CarrierSource = value;
                OnPropertyChanged();
            }
        }

        public void SelectCarrierImage()
        {
            string bitmapName = Utils.GetBitmap();

            if (bitmapName != string.Empty)
                CarrierSource = new BitmapImage(new Uri(bitmapName));
        }


        public ICommand HideMessageCommand { get; set; }

        public void HideMessage()
        {
            if (string.IsNullOrEmpty(SecretMessage))
            {
                Error("You must enter the message!");
                return;
            }

            if (string.IsNullOrEmpty(HidePassword))
            {
                Error("You must enter the password!");
                return;
            }

            if (CarrierSource == null)
            {
                Error("You must select the carrier image!");
                return;
            }

            string carrierImageName = CarrierSource.ToString().Replace("file:///", string.Empty);
            string resultImageName = "ImageWithSecretMessage_" + Utils.Timestamp() + ".bmp";

            var result = SteganographyHelper.HideSecretMessage(SecretMessage, HidePassword, carrierImageName, resultImageName);

            if (result.Success)
            {
                Success("The image with the secret message was saved in the current folder with the name " + resultImageName);
                SecretMessage = string.Empty;
                HidePassword = string.Empty;
                CarrierSource = null;
            }
            else
            {
                Error(result.Message);
            }
        }


        private string _RecoverPassword;
        public string RecoverPassword
        {
            get => _RecoverPassword;
            set
            {
                _RecoverPassword = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _ResultSource;
        public ImageSource ResultSource
        {
            get => _ResultSource;
            set
            {
                _ResultSource = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectResultImageCommand { get; set; }

        public void SelectResultImage()
        {
            string bitmapName = Utils.GetBitmap();

            if (bitmapName != string.Empty)
                ResultSource = new BitmapImage(new Uri(bitmapName));
        }

        public ICommand RecoverMessageCommand { get; set; }

        public void RecoverMessage()
        {
            if (string.IsNullOrEmpty(RecoverPassword))
            {
                Error("You must enter the recovery password!");
                return;
            }

            if (ResultSource == null)
            {
                Error("You must select the image containing the secret message");
                return;
            }

            string imageWithMessageName = ResultSource.ToString().Replace("file:///", string.Empty);

            var result = SteganographyHelper.RecoverSecretMessage(RecoverPassword, imageWithMessageName);


            if (result.Success)
            {
                RecoveredMessage = result.Value;
            }
            else
            {
                Error(result.Message);
            }
        }

        private string _RecoveredMessage;
        public string RecoveredMessage
        {
            get => _RecoveredMessage;
            set
            {
                _RecoveredMessage = value;
                OnPropertyChanged();
            }
        }


    }
}

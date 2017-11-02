using LSBSteganography.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LSBSteganography.ViewModel
{
    public class RecoverBitmapViewModel : ViewModelBase
    {
        public RecoverBitmapViewModel()
        {
            SelectResultImageCommand = new RelayCommand(SelectResultImage);
            RecoverCommand = new RelayCommand(Recover);
            OneBit = true;
        }

        public bool OneBit { get; set; }
        public bool TwoBits { get; set; }
        public bool ThreeBits { get; set; }

        private ImageSource _ResultSource;
        public ImageSource ResultSource
        {
            get =>  _ResultSource;
            set
            {
                _ResultSource = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _RecoveredSecretSource;
        public ImageSource RecoveredSecretSource
        {
            get => _RecoveredSecretSource;
            set
            {
                _RecoveredSecretSource = value;
                OnPropertyChanged();
            }
        }


        public ICommand SelectResultImageCommand { get; set; }

        public void SelectResultImage()
        {
            string bitmapName = Utils.GetBitmap();

            if (bitmapName != string.Empty)
            {
                ResultSource = new BitmapImage(new Uri(bitmapName));
                RecoveredSecretSource = null;
            }
        }



        public ICommand RecoverCommand { get; set; }

        public void Recover()
        {
            if (ResultSource == null)
            {
                Error("You must select the image that contains the secret");
                return;
            }

            int numberOfBits = OneBit ? 1 : TwoBits ? 2 : ThreeBits ? 3 : 1;

            string resultName = ResultSource.ToString().Replace("file:///", string.Empty);
            string recoveredImageName = "recoveredImage_" + Utils.Timestamp() + ".bmp";

            Result result = SteganographyHelper.RecoverSecretBitmap(resultName, recoveredImageName, numberOfBits);

            if (result.Success)
            {
                RecoveredSecretSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\" + recoveredImageName));
                Success("Success. The resulting image was saved in the current folder with the name " + recoveredImageName);
            }
            else
            {
                Error(result.Message);
            }
        }
    }
}

using LSBSteganography.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LSBSteganography.ViewModel
{
    public class HideBitmapViewModel : ViewModelBase
    {
        public HideBitmapViewModel()
        {
            SelectCarrierImageCommand = new RelayCommand(SelectCarrierImage);
            SelectSecretImageCommand = new RelayCommand(SelectSecretImage);
            HideCommand = new RelayCommand(Hide);
            OneBit = true;
        }


        public bool OneBit { get; set; }
        public bool TwoBits { get; set; }
        public bool ThreeBits { get; set; }

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

        private ImageSource _SecretSource;
        public ImageSource SecretSource
        {
            get => _SecretSource;
            set
            {
                _SecretSource = value;
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

        public ICommand SelectCarrierImageCommand { get; set; }

        public void SelectCarrierImage()
        {
            string bitmapName = Utils.GetBitmap();

            if (bitmapName != string.Empty)
            {
                CarrierSource = new BitmapImage(new Uri(bitmapName));
                ResultSource = null;
            }
        }


        public ICommand SelectSecretImageCommand { get; set; }

        public void SelectSecretImage()
        {
            string bitmapName = Utils.GetBitmap();

            if (bitmapName != string.Empty)
            {
                SecretSource = new BitmapImage(new Uri(bitmapName));
                ResultSource = null;
            }
        }


        public ICommand HideCommand { get; set; }

        public void Hide()
        {
            if (CarrierSource == null || SecretSource == null)
            {
                Error("You must select both the carrier and the secret");
                return;
            }

            int numberOfBits = OneBit ? 1 : TwoBits ? 2 : ThreeBits ? 3 : 1;

            string carrierName = CarrierSource.ToString().Replace("file:///", string.Empty);
            string secretName = SecretSource.ToString().Replace("file:///", string.Empty);
            string resultName = "Result_" + Utils.Timestamp() + ".bmp";

            var result = SteganographyHelper.HideSecretBitmap(carrierName, secretName, resultName, numberOfBits);

            if (result.Success)
            {
                ResultSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\" + resultName));
                Success("Success. The resulting image was saved in the current folder with the name " + resultName);
            }
            else
            {
                Error(result.Message);
            }
        }
    }
}

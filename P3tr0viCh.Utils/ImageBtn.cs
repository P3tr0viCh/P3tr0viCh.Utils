using System.Windows.Forms;
using System;
using P3tr0viCh.Utils.Extensions;

namespace P3tr0viCh.Utils
{
    public class ImageBtn
    {
        public PictureBox Button { get; private set; }

        private int imageNormal;
        public int ImageNormal
        {
            get
            {
                return imageNormal;
            }
            set
            {
                imageNormal = value;

                UpdateImage(!Button.MouseIsOverControl());
            }
        }
        private int imageHover;
        public int ImageHover
        {
            get
            {
                return imageHover;
            }
            set
            {
                imageHover = value;

                UpdateImage(!Button.MouseIsOverControl());
            }
        }

        public void SetImages(int imageNormal, int imageHover)
        {
            this.imageNormal = imageNormal;
            this.imageHover = imageHover;

            UpdateImage(!Button.MouseIsOverControl());
        }

        private ImageList imageList;
        public ImageList ImageList
        {
            get { return imageList; }
            set
            {
                imageList = value;

                UpdateImage(!Button.MouseIsOverControl());
            }
        }

        public ImageBtn(PictureBox button, ImageList imageList, int imageNormal, int imageHover)
        {
            Button = button;

            this.imageList = imageList;

            SetImages(imageNormal, imageHover);

            UpdateImage(!Button.MouseIsOverControl());

            Button.MouseEnter += Button_MouseEnter;
            Button.MouseLeave += Button_MouseLeave;
        }

        private void UpdateImage(bool normalState)
        {
            Button.Image = ImageList.Images[normalState ? ImageNormal : ImageHover];
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            UpdateImage(true);
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            UpdateImage(false);
        }
    }
}
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Entities;

namespace ForwardLayoutTest
{
    public class CanvasImage
    {
        public Position position = new Position();
        public Position end_position = new Position();

        public Dimensions dimensions = new Dimensions();

        public int source_hashcode = 0;

        public string image_id = "";

        public CanvasImage() {}

        public void update(Image original_image, Image image)
        {
            dimensions.width = image.ActualWidth;
            dimensions.height = image.ActualHeight;

            position.x = Canvas.GetLeft(image);
            position.y = Canvas.GetTop(image);

            end_position.x = position.x + dimensions.width;
            end_position.y = position.y + dimensions.height;
        }
    }
}

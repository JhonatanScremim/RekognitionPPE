using Amazon.Rekognition.Model;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RekognitionPPE
{
    public class DrawBox
    {
        public void DrawPerson(ProtectiveEquipmentPerson item, PictureBox picture)
        {
            var pen = new Pen(Color.Red);
            var g = picture.CreateGraphics();
            g.DrawRectangle(pen,
                item.BoundingBox.Left * picture.Image.Width,
                item.BoundingBox.Top * picture.Image.Height,
                item.BoundingBox.Width * picture.Image.Width,
                item.BoundingBox.Height * picture.Image.Height);
        }

        public void DrawEquipment(EquipmentDetection item, PictureBox picture)
        {
            var pen = new Pen(Color.Red);
            var g = picture.CreateGraphics();
            g.DrawRectangle(pen,
                item.BoundingBox.Left * picture.Image.Width,
                item.BoundingBox.Top * picture.Image.Height,
                item.BoundingBox.Width * picture.Image.Width,
                item.BoundingBox.Height * picture.Image.Height);
        }
    }
}

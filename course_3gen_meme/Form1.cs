using System;
using System.Drawing;
using System.Windows.Forms;

namespace course_3gen_meme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Setsize();//инициализация bitmap и graphics
            var pos = this.PointToScreen(label4.Location);//переменная места метки
            pos = pictureBox1.PointToClient(pos);//присвоение пикчебоксу
            label4.Parent = pictureBox1;
            label4.Location = pos;// на позицию пикчебокса
            label4.BackColor = Color.Transparent;//прозрачность заднего фона надписи
        }
        private class Arraypoints
        {
            private int index = 0;//номер текущей точки0 в массиве
            private Point[] points;//массив точек

            public Arraypoints(int size)//конструктор для задания размера
            {
                if (size <= 0) size = 2;// size - размер массива
                points = new Point[size];//инициализация массива
            }
            public void Setpoints (int x, int y)//принимает координаты точки
            {
                if (index >= points.Length)//проверка номера точки, не выходит ли за границы
                    index = 0;
                points[index] = new Point(x, y);//присвоение новой точки
                index++;
            }
            public void resetpoints()//сброс точек, при отпускании лкм
            {
                index = 0;
            }
            public int getcountpoints()//возврат текущего индекса, сколько точек в массиве
            {
                return index;
            }
            public Point[] getpoints()//возврат массива точек, для отрисовки
            {
                return points;
            }
        }

        private bool isMouse = false;//проверка на зажатую лкм
        private Arraypoints arrayPoints = new Arraypoints(2); //обьект массива из двух точек
        Bitmap bitmap = new Bitmap(100, 100);//отвечает за хранение изображения
        Graphics graphics;
        Pen pen = new Pen(Color.Black, 3f);//обьект кисти
        private object currentlabel = null;// выбор метки

        // задача размера для bitmap
        private void Setsize()
        {
            Rectangle rectangle = Screen.PrimaryScreen.Bounds;//определение разрешения пользователя
            bitmap = new Bitmap(rectangle.Width, rectangle.Height);//передача в bitmap
            graphics = Graphics.FromImage(bitmap);// передача переменной bitmap
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;//сглаживание линии
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;//сглаживание линии
        }
        //при нажатии мыши переводит isMouse в состояние true
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouse = true;
        }
        //при отжиме кнопки мыши переводит isMouse в состояние false и сбрасывает массив точек
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouse = false;
            arrayPoints.resetpoints();
        }
        // выбор цвета кисти из палитры цветов
        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                pen.Color = colorDialog1.Color;
            ((Button)sender).BackColor = colorDialog1.Color;
        }
        //кнопка очистки
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            label4.BackColor = Color.White;
            label4.Text = null;
            graphics.Clear(pictureBox1.BackColor);
            pictureBox1.Image = bitmap;
        }
        // отвечает за толшину линии кисти
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            pen.Width = trackBar1.Value;
        }
        // сохранение рисунка
        private void button1_Click(object sender, EventArgs e)
        {
            graphics.DrawString(label4.Text, new Font("Arial", 14), Brushes.Black, label4.Location.X, label4.Location.Y);
            label4.Text = null;
            saveFileDialog1.Filter = "JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|Все файлы(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                if (pictureBox1.Image != null)
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
        }
        
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouse) { return; } //если мышка не нажата, ничего не делать
            arrayPoints.Setpoints(e.X, e.Y); //установка начала линии, заполнение массива
            if (arrayPoints.getcountpoints() >= 2)//проверка на к-во точек
            {
                graphics.DrawLines(pen, arrayPoints.getpoints());// рисует
                pictureBox1.Image = bitmap;//присвоение рисунка
                arrayPoints.Setpoints(e.X, e.Y);//чтоб линия не прирывалась 
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (currentlabel != null) //проверка выбраной метки
            {   // oчистка
                textBox1.Clear();
                label4.BackColor = Color.White;
                label4.Text = null;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            OpenFileDialog name = new OpenFileDialog();
            name.Filter = "JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|Все файлы(*.*)|*.*";
            if(name.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(name.FileName);
                    graphics.DrawImage(pictureBox1.Image, pictureBox1.Location.X, pictureBox1.Location.Y);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch
                {
                    MessageBox.Show("can not open file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            isMouse = true;
        }

        private void label4_MouseUp(object sender, MouseEventArgs e)
        {
            isMouse = false;
        }

        private void label4_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouse) { return; } //если мышка не нажата, ничего не делать
            if (currentlabel != null) //проверка выбраной метки
                currentlabel.GetType().GetProperty("Location").SetValue(currentlabel, new Point(Cursor.Position.X - 100, Cursor.Position.Y - 100));// перемещение выбраной метки
        }

        private void label4_MouseClick(object sender, MouseEventArgs e)//для работы с выбраной меткой
        {
            currentlabel = sender;
        }
        // выбор шрифта и размера
        private void button4_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() != DialogResult.OK) { return; }// задание шрифта
            label4.Font = fontDialog1.Font;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.OK)
            {
                label4.BackColor = colorDialog2.Color; //задание цвета
                textBox1.BackColor = colorDialog2.Color;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label4.Text = textBox1.Text; //отображение текста на метке
            pictureBox1.Image = bitmap;// отображение текста
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://drive.google.com/drive/folders/1tcQEaoZilB53W82dU2-SsbP9eHTgUGZh?usp=sharing");
            }
            catch
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }
        // кнопка цвета текста
        private void button7_Click(object sender, EventArgs e)
        {
            if (colorDialog3.ShowDialog() == DialogResult.OK)
            {
                label4.ForeColor = colorDialog3.Color; //задание цвета
                textBox1.ForeColor = colorDialog3.Color;
            }
        }
    }
}

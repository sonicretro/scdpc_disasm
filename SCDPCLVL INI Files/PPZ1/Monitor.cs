using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SonicRetro.SonLVL;

namespace SCDPCObjectDefinitions.Common
{
    class Monitor : ObjectDefinition
    {
        private Point offset;
        private BitmapBits img;
        private int imgw, imgh;
		private BitmapBits postimg;
		private Point postoff;
        private List<Point> offsets = new List<Point>();
        private List<BitmapBits> imgs = new List<BitmapBits>();
        private List<int> imgws = new List<int>();
        private List<int> imghs = new List<int>();

        public override void Init(Dictionary<string, string> data)
        {
            img = ObjectHelper.Sprite(286, out offset);
            imgw = img.Width;
            imgh = img.Height;
			postimg = ObjectHelper.Sprite(285, out postoff);
            Point off;
            BitmapBits im;
            for (int i = 0; i < 8; i++)
            {
				im = ObjectHelper.Sprite(272 + i, out off);
                imgs.Add(im);
                offsets.Add(off);
                imgws.Add(im.Width);
                imghs.Add(im.Height);
            }
            for (int i = 0; i < 2; i++)
            {
				im = ObjectHelper.Sprite(281 + i, out off);
                imgs.Add(im);
                offsets.Add(off);
                imgws.Add(im.Width);
                imghs.Add(im.Height);
            }
        }

        public override ReadOnlyCollection<byte> Subtypes()
        {
            return new ReadOnlyCollection<byte>(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        }

        public override string Name()
        {
            return "Monitor/Timepost";
        }

        public override bool RememberState()
        {
            return true;
        }

        public override string SubtypeName(byte subtype)
        {
            switch (subtype)
            {
                case 0:
                    return "Sonic Monitor";
                case 1:
                    return "Rings Monitor";
                case 2:
                    return "Shield Monitor";
                case 3:
                    return "Invincibility Monitor";
                case 4:
                    return "Speed Shoes Monitor";
                case 5:
                    return "Clock Monitor";
                case 6:
                    return "Silver Ring Monitor";
                case 7:
                    return "S Monitor";
                case 8:
                    return "Past Timepost";
                case 9:
                    return "Future Timepost";
                default:
                    return "Unknown";
            }
        }

        public override string FullName(byte subtype)
        {
            return SubtypeName(subtype);
        }

        public override BitmapBits Image()
        {
            return img;
        }

        public override BitmapBits Image(byte subtype)
        {
            if (subtype <= 9)
                return imgs[subtype];
            else
                return img;
        }

        public override Rectangle Bounds(Point loc, byte subtype)
        {
            if (subtype <= 7)
                return Rectangle.Union(new Rectangle(loc.X + offset.X, loc.Y + offset.Y, imgw, imgh), new Rectangle(loc.X + offsets[subtype].X, loc.Y + offsets[subtype].Y, imgws[subtype], imghs[subtype]));
            else if (subtype <= 9)
                return Rectangle.Union(new Rectangle(loc.X + postoff.X, loc.Y + postoff.Y, postimg.Width, postimg.Height), new Rectangle(loc.X + offsets[subtype].X, loc.Y + offsets[subtype].Y, imgws[subtype], imghs[subtype]));
			else
                return new Rectangle(loc.X + offset.X, loc.Y + offset.Y, imgw, imgh);
        }

        public override void Draw(BitmapBits bmp, Point loc, byte subtype, bool XFlip, bool YFlip, bool includeDebug)
        {
            if (subtype <= 7 | subtype > 9)
            {
    			BitmapBits bits = new BitmapBits(img);
	            bits.Flip(XFlip, YFlip);
                bmp.DrawBitmapComposited(bits, new Point(loc.X + offset.X, loc.Y + offset.Y));
				if (subtype <= 7)
				{
				    bits = new BitmapBits(imgs[subtype]);
    	            bits.Flip(XFlip, YFlip);
                    bmp.DrawBitmapComposited(bits, new Point(loc.X + offsets[subtype].X, loc.Y + offsets[subtype].Y));
				}
		    }
			else
			{
    			BitmapBits bits = new BitmapBits(postimg);
	            bits.Flip(XFlip, YFlip);
                bmp.DrawBitmapComposited(bits, new Point(loc.X + postoff.X, loc.Y + postoff.Y));
    		    bits = new BitmapBits(imgs[subtype]);
                bits.Flip(XFlip, YFlip);
                bmp.DrawBitmapComposited(bits, new Point(loc.X + offsets[subtype].X, loc.Y + offsets[subtype].Y));
    		}
		}

        public override Type ObjectType
        {
            get
            {
                return typeof(MonitorSCDObjectEntry);
            }
        }
    }

    public class MonitorSCDObjectEntry : SCDObjectEntry
    {
        public MonitorSCDObjectEntry() : base() { }
        public MonitorSCDObjectEntry(byte[] file, int address) : base(file, address) { }

        public MonitorType Type
        {
            get
            {
                return (MonitorType)(SubType & 8);
            }
            set
            {
                SubType = (byte)((SubType & ~8) | (int)value);
            }
        }
		
		public enum MonitorType
		{
		    Monitor = 0,
			Timepost = 8
		}
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SonicRetro.SonLVL;

namespace SCDPCObjectDefinitions.PPZ1
{
    class Spring : ObjectDefinition
    {
        private List<Point> offsets = new List<Point>();
        private List<BitmapBits> imgs = new List<BitmapBits>();

        public override void Init(Dictionary<string, string> data)
        {
            Point off = new Point();
            imgs.Add(ObjectHelper.Sprite(479, out off)); // 0x00
            offsets.Add(off);
            imgs.Add(ObjectHelper.Sprite(467, out off)); // 0x02
            offsets.Add(off);
            imgs.Add(ObjectHelper.Sprite(482, out off)); // 0x10
            offsets.Add(off);
            imgs.Add(ObjectHelper.Sprite(470, out off)); // 0x12
            offsets.Add(off);
            imgs.Add(ObjectHelper.Sprite(485, out off)); // 0x20
            offsets.Add(off);
            imgs.Add(ObjectHelper.Sprite(473, out off)); // 0x22
            offsets.Add(off);
            imgs.Add(imgs[0]); // 0x30
            offsets.Add(offsets[0]);
            imgs.Add(imgs[1]); // 0x32
            offsets.Add(offsets[1]);
        }

        public override ReadOnlyCollection<byte> Subtypes()
        {
            return new ReadOnlyCollection<byte>(new byte[] { 0x00, 0x02, 0x10, 0x12, 0x20, 0x22 });
        }

        public override string Name()
        {
            return "Spring";
        }

        public override bool RememberState()
        {
            return false;
        }

        public override string SubtypeName(byte subtype)
        {
            return ((SpringDirection)((subtype & 0x30) >> 4)).ToString() + " " + ((SpringColor)((subtype & 2) >> 1)).ToString();
        }

        public override string FullName(byte subtype)
        {
            return SubtypeName(subtype) + " " + Name();
        }

        public override BitmapBits Image()
        {
            return imgs[0];
        }

        private int imgindex(byte subtype)
        {
            int result = (subtype & 2) >> 1;
            result |= (subtype & 0x30) >> 3;
            return result;
        }

        public override BitmapBits Image(byte subtype)
        {
            return imgs[imgindex(subtype)];
        }

        public override Rectangle Bounds(Point loc, byte subtype)
        {
            return new Rectangle(loc.X + offsets[imgindex(subtype)].X, loc.Y + offsets[imgindex(subtype)].Y, imgs[imgindex(subtype)].Width, imgs[imgindex(subtype)].Height);
        }

        public override void Draw(BitmapBits bmp, Point loc, byte subtype, bool XFlip, bool YFlip, bool includeDebug)
        {
            BitmapBits bits = new BitmapBits(imgs[imgindex(subtype)]);
            bits.Flip(XFlip, YFlip);
            bmp.DrawBitmapComposited(bits, new Point(loc.X + offsets[imgindex(subtype)].X, loc.Y + offsets[imgindex(subtype)].Y));
        }

        public override Type ObjectType
        {
            get
            {
                return typeof(SpringSCDObjectEntry);
            }
        }
    }

    public class SpringSCDObjectEntry : SCDObjectEntry
    {
        public SpringSCDObjectEntry() : base() { }
        public SpringSCDObjectEntry(byte[] file, int address) : base(file, address) { }

        public SpringDirection Direction
        {
            get
            {
                return (SpringDirection)((SubType & 0x30) >> 4);
            }
            set
            {
                SubType = (byte)((SubType & ~0x30) | ((int)value << 4));
            }
        }

        public SpringColor Color
        {
            get
            {
                return (SpringColor)((SubType & 2) >> 1);
            }
            set
            {
                SubType = (byte)((SubType & ~2) | ((int)value << 1));
            }
        }
    }

    public enum SpringDirection
    {
        Vertical,
        Horizontal,
        Diagonal,
        Invalid
    }

    public enum SpringColor
    {
        Red,
        Yellow
    }
}
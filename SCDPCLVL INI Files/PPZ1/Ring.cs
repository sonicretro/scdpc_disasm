using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SonicRetro.SonLVL;

namespace SCDPCObjectDefinitions.PPZ1
{
    public class Ring : ObjectDefinition
    {
        private Size[] Spacing = {
                                     new Size(0x10, 0), // horizontal tight
                                     new Size(0x18, 0), // horizontal normal
                                     new Size(0x20, 0), // horizontal wide
                                     new Size(0, 0x10), // vertical tight
                                     new Size(0, 0x18), // vertical normal
                                     new Size(0, 0x20), // vertical wide
                                     new Size(0x10, 0x10), // diagonal
                                     new Size(0x18, 0x18),
                                     new Size(0x20, 0x20),
                                     new Size(-0x10, 0x10),
                                     new Size(-0x18, 0x18),
                                     new Size(-0x20, 0x20),
                                     new Size(0x10, 8),
                                     new Size(0x18, 0x10),
                                     new Size(-0x10, 8),
                                     new Size(-0x18, 0x10)
                                 };

        private Point offset;
        private BitmapBits img;
        public override void Init(Dictionary<string, string> data)
        {
            img = ObjectHelper.Sprite(362, out offset);
        }

        public override ReadOnlyCollection<byte> Subtypes()
        {
            return new ReadOnlyCollection<byte>(new List<byte>());
        }

        public override string Name()
        {
            return "Ring";
        }

        public override bool RememberState()
        {
            return true;
        }

        public override string SubtypeName(byte subtype)
        {
            return string.Empty;
        }

        public override string FullName(byte subtype)
        {
            return Name();
        }

        public override BitmapBits Image()
        {
            return img;
        }

        public override BitmapBits Image(byte subtype)
        {
            return img;
        }

        public override Rectangle Bounds(Point loc, byte subtype)
        {
            int count = Math.Min(6, subtype & 7);
            Size space = Spacing[subtype >> 4];
            return new Rectangle(loc.X + offset.X, loc.Y + offset.Y, (space.Width * count) + img.Width, (space.Height * count) + img.Height);
        }

        public override void Draw(BitmapBits bmp, Point loc, byte subtype, bool XFlip, bool YFlip, bool includeDebug)
        {
            BitmapBits bits = new BitmapBits(img);
            int count = Math.Min(6, subtype & 7) + 1;
            Size space = Spacing[subtype >> 4];
            for (int i = 0; i < count; i++)
            {
                bmp.DrawBitmapComposited(bits, new Point(loc.X + offset.X, loc.Y + offset.Y));
                loc += space;
            }
        }

        public override Type ObjectType
        {
            get
            {
                return typeof(RingSCDObjectEntry);
            }
        }
    }

    public class RingSCDObjectEntry : SCDObjectEntry
    {
        public RingSCDObjectEntry() : base() { }
        public RingSCDObjectEntry(byte[] file, int address) : base(file, address) { }

        public int Count
        {
            get
            {
                return Math.Min(6, SubType & 7) + 1;
            }
            set
            {
                SubType = (byte)((SubType & ~7) | (Math.Min(value, 7) - 1));
            }
        }

        public int Direction
        {
            get
            {
                return SubType >> 4;
            }
            set
            {
                SubType = (byte)((SubType & ~0xF0) | ((value & 0xF) << 4));
            }
        }
    }
}
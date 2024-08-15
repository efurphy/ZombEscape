using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ZombEscape
{
    internal static class Assets
    {
        public static AssetHolder<Texture2D> sprites;
        public static AssetHolder<BitmapFont> fonts;

        static Assets()
        {
            sprites = new();
            fonts = new();
        }

        public static void Load(ContentManager content)
        {
            sprites.Add("player", content.Load<Texture2D>("sprites/circle"));
            sprites.Add("zombie", content.Load<Texture2D>("sprites/circle"));
            sprites.Add("button", content.Load<Texture2D>("sprites/button"));

            fonts.Add("small",   content.Load<BitmapFont>("fonts/fnt_small"));
            fonts.Add("medium",  content.Load<BitmapFont>("fonts/fnt_medium"));
            fonts.Add("large",   content.Load<BitmapFont>("fonts/fnt_large"));
        }
    }
}

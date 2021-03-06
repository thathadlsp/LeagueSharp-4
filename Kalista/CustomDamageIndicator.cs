﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

using SharpDX;

using Color = System.Drawing.Color;

namespace Kalista
{
    // Kinda credits to detuks (http://www.joduska.me/forum/user/107-/)
    // Removed unneeded stuff and improved the code to my needs
    public class CustomDamageIndicator
    {
        private const int BAR_WIDTH = 104;
        private const int LINE_THICKNESS = 4;

        private static Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit;

        private static readonly Vector2 BarOffset = new Vector2(10, 25);

        private static System.Drawing.Color _drawingColor;
        public static System.Drawing.Color DrawingColor
        {
            get { return _drawingColor; }
            set { _drawingColor = Color.FromArgb(80, value); }
        }

        public static void Initialize(Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit)
        {
            // Apply needed field delegate for damage calculation
            CustomDamageIndicator.damageToUnit = damageToUnit;
            DrawingColor = System.Drawing.Color.OrangeRed;

            // Register event handlers
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(u => u.IsValidTarget() && u.IsHPBarRendered))
            {
                // Get damage to unit
                var damage = damageToUnit(unit);

                // Continue on 0 damage
                if (damage <= 0)
                    continue;

                // Get remaining HP after damage applied in percent and the current percent of health
                var damagePercentage = ((unit.Health - damage) > 0 ? (unit.Health - damage) : 0) / unit.MaxHealth;
                var currentHealthPercentage = unit.Health / unit.MaxHealth;

                // Calculate start and end point of the bar indicator
                var startPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + damagePercentage * BAR_WIDTH), (int)(unit.HPBarPosition.Y + BarOffset.Y) + 4);
                var endPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage * BAR_WIDTH) + 1, (int)(unit.HPBarPosition.Y + BarOffset.Y) + 4);

                // Draw the line
                Drawing.DrawLine(startPoint, endPoint, LINE_THICKNESS, DrawingColor);
            }
        }
    }
}

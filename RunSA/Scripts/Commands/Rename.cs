using System;
using System.Reflection;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Items;
using Server.Gumps;
using CPA = Server.CommandPropertyAttribute;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;

namespace Server.Commands
{
    public class Rename
    {
	private static string NameArg = "Nothing";
        public static void Initialize()
        {
            CommandSystem.Register("Rename", AccessLevel.Player, new CommandEventHandler(Rename_OnCommand));
        }
		[Usage( "Rename <name>" )]
		[Description( "Sets the name of a targeted mobile." )]
		private class RenameTarget : Target
		{
			public RenameTarget() : base( 16, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( !BaseCommand.IsAccessible( from, o ) )
					from.SendMessage( "That is not accessible." );
				else
				{
				try
				{
					BaseCreature bc = o as BaseCreature;
					if (bc.ControlMaster == from)
					{
						bc.Name = NameArg;
					}
					else
						from.SendMessage( "You can't name that!" );
				}
				catch
				{
					from.SendMessage( "You can't name that!" );
				}
				}
			}
		}

        [Usage("Rename [serial]")]
        [Description("Opens a menu where you can view and edit all properties of a targeted (or specified) object.")]
        private static void Rename_OnCommand(CommandEventArgs e)
        {
	     NameArg = e.GetString( 0 );
             e.Mobile.Target = new RenameTarget();
        }
    }
}
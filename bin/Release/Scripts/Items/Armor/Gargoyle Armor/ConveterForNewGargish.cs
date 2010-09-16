using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands
{
	public class UpdateGargishArmorCommand
	{
		public static Dictionary<Type, Type> TypeList = new Dictionary<Type, Type>();

		public static void Configure()
		{
			// Leather
			TypeList.Add( typeof( MaleGargishLeatherChest ), typeof( GargishLeatherChest ) );
			TypeList.Add( typeof( FemaleGargishLeatherChest ), typeof( GargishLeatherChest ) );
			TypeList.Add( typeof( MaleGargishLeatherArms ), typeof( GargishLeatherArms ) );
			TypeList.Add( typeof( FemaleGargishLeatherArms ), typeof( GargishLeatherArms ) );
			TypeList.Add( typeof( MaleGargishLeatherKilt ), typeof( GargishLeatherKilt ) );
			TypeList.Add( typeof( FemaleGargishLeatherKilt ), typeof( GargishLeatherKilt ) );
			TypeList.Add( typeof( MaleGargishLeatherLegs ), typeof( GargishLeatherLegs ) );
			TypeList.Add( typeof( FemaleGargishLeatherLegs ), typeof( GargishLeatherLegs ) );

			// Plate
			TypeList.Add( typeof( MaleGargishPlateChest ), typeof( GargishPlateChest ) );
			TypeList.Add( typeof( FemaleGargishPlateChest ), typeof( GargishPlateChest ) );
			TypeList.Add( typeof( MaleGargishPlateArms ), typeof( GargishPlateArms ) );
			TypeList.Add( typeof( FemaleGargishPlateArms ), typeof( GargishPlateArms ) );
			TypeList.Add( typeof( MaleGargishPlateKilt ), typeof( GargishPlateKilt ) );
			TypeList.Add( typeof( FemaleGargishPlateKilt ), typeof( GargishPlateKilt ) );
			TypeList.Add( typeof( MaleGargishPlateLegs ), typeof( GargishPlateLegs ) );
			TypeList.Add( typeof( FemaleGargishPlateLegs ), typeof( GargishPlateLegs ) );

			// Stone
			TypeList.Add( typeof( MaleGargishStoneChest ), typeof( GargishStoneChest ) );
			TypeList.Add( typeof( FemaleGargishStoneChest ), typeof( GargishStoneChest ) );
			TypeList.Add( typeof( MaleGargishStoneArms ), typeof( GargishStoneArms ) );
			TypeList.Add( typeof( FemaleGargishStoneArms ), typeof( GargishStoneArms ) );
			TypeList.Add( typeof( MaleGargishStoneKilt ), typeof( GargishStoneKilt ) );
			TypeList.Add( typeof( FemaleGargishStoneKilt ), typeof( GargishStoneKilt ) );
			TypeList.Add( typeof( MaleGargishStoneLegs ), typeof( GargishStoneLegs ) );
			TypeList.Add( typeof( FemaleGargishStoneLegs ), typeof( GargishStoneLegs ) );

			// Cloth
			TypeList.Add( typeof( MaleGargishClothChest ), typeof( GargishClothChest ) );
			TypeList.Add( typeof( FemaleGargishClothChest ), typeof( GargishClothChest ) );
			TypeList.Add( typeof( MaleGargishClothArms ), typeof( GargishClothArms ) );
			TypeList.Add( typeof( FemaleGargishClothArms ), typeof( GargishClothArms ) );
			TypeList.Add( typeof( MaleGargishClothKilt ), typeof( GargishClothKilt ) );
			TypeList.Add( typeof( FemaleGargishClothKilt ), typeof( GargishClothKilt ) );
			TypeList.Add( typeof( MaleGargishClothLegs ), typeof( GargishClothLegs ) );
			TypeList.Add( typeof( FemaleGargishClothLegs ), typeof( GargishClothLegs ) );
		}

		public static void Initialize()
		{
			CommandSystem.Register( "UpdateGargishArmor", AccessLevel.Owner, new CommandEventHandler( UpdateGargishArmor_OnCommand ) );
		}

		[Description( "Update all armor in the game to the new style of gargish armor" )]
		public static void UpdateGargishArmor_OnCommand( CommandEventArgs e )
		{
			Mobile m = null;
			Container c = null;
			List<Item> items = new List<Item>();

			foreach( Item itemsintheworld in World.Items.Values )
			{
				if ( itemsintheworld == null )
					continue;

				if ( TypeList.ContainsKey( itemsintheworld.GetType() ) )
					items.Add( itemsintheworld );
			}

			for( int i = 0; i < items.Count; i++ )
			{
				Item item = items[i];

				if ( item == null )
					continue;

				// already checked before adding them to the list.
				// if ( !TypeList.ContainsKey( item.GetType() ) )
					// continue;

				Item newitem = GargishConverter.CreateItem( TypeList[item.GetType()] );

				if ( newitem == null )
				{
					Console.WriteLine( "You spin me right round baby right round, like a record baby right round, round, round. :p" ); // You know something is wrong when your computer starts dick rolling you >.>
					continue;
				}

				newitem.Map = item.Map;

				if ( item.Parent is Mobile )
				{
					m = (Mobile)item.Parent;

					if ( m.Backpack != null && m.Backpack is Container )
					{
						c = (Container)m.Backpack;
						c.DropItem( newitem );
					}
					else // No packpack? Probably a creature, skip it.
					{
						newitem.Delete();
					}
				}
				else if ( item.Parent is Container )
				{
					c = (Container)item.Parent;
					c.DropItem( newitem );
				}

				if ( newitem != null && !(item.Parent is Mobile) )
				{
					newitem.X = item.X;
					newitem.Y = item.Y;
					newitem.Z = item.Z;
				}

				item.Delete();
			}
		}

	}
}
/*



*/
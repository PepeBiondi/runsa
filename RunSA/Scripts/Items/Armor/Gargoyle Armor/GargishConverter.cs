using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server
{
	public class GargishConverter
	{
		public static Dictionary<Type, Type> TypeList = new Dictionary<Type, Type>();

		public static void Configure()
		{
			// Weapons
			TypeList.Add( typeof( VikingSword ), typeof( StoneWarSword ) );
			TypeList.Add( typeof( BoneHarvester ), typeof( GlassSword ) );
			TypeList.Add( typeof( ElvenMachete ), typeof( GlassSword ) );
			TypeList.Add( typeof( Katana ), typeof( GlassSword ) );
			TypeList.Add( typeof( Cutlass ), typeof( GlassSword ) );
			TypeList.Add( typeof( Cleaver ), typeof( GlassSword ) );
			TypeList.Add( typeof( RadiantScimitar ), typeof( GlassSword ) );
			TypeList.Add( typeof( Scimitar ), typeof( GlassSword ) );
			TypeList.Add( typeof( Longsword ), typeof( DreadSword ) );
			TypeList.Add( typeof( Broadsword ), typeof( DreadSword ) );
			TypeList.Add( typeof( Bardiche ), typeof( GargishTalwar ) );
			TypeList.Add( typeof( Halberd ), typeof( GargishTalwar ) );
			TypeList.Add( typeof( NoDachi ), typeof( GargishTalwar ) );
			TypeList.Add( typeof( Daisho ), typeof( GargishTalwar ) );
			TypeList.Add( typeof( RuneBlade ), typeof( GargishTalwar ) );
			TypeList.Add( typeof( CrescentBlade ), typeof( GargishTalwar ) );
			TypeList.Add( typeof( Scythe ), typeof( GargishTalwar ) );
			TypeList.Add( typeof( WarFork ), typeof( Shortblade ) );
			TypeList.Add( typeof( WarCleaver ), typeof( Shortblade ) );
			TypeList.Add( typeof( AssassinSpike ), typeof( Shortblade ) );
			TypeList.Add( typeof( Kryss ), typeof( BloodBlade ) );
			TypeList.Add( typeof( WarHammer ), typeof( DiscMace ) );
			TypeList.Add( typeof( WarMace ), typeof( DiscMace ) );
			TypeList.Add( typeof( Mace ), typeof( DiscMace ) );
			TypeList.Add( typeof( Maul ), typeof( DiscMace ) );
			TypeList.Add( typeof( Scepter ), typeof( DiscMace ) );
			TypeList.Add( typeof( Club ), typeof( DiscMace ) );
			TypeList.Add( typeof( HammerPick ), typeof( DiscMace ) );
			TypeList.Add( typeof( DiamondMace ), typeof( DiscMace ) );
			TypeList.Add( typeof( WildStaff ), typeof( DiscMace ) );
			TypeList.Add( typeof( WarAxe ), typeof( DiscMace ) );
			TypeList.Add( typeof( Tetsubo ), typeof( GlassStaff ) );
			TypeList.Add( typeof( Tessen ), typeof( GlassStaff ) );
			TypeList.Add( typeof( BlackStaff ), typeof( GlassStaff ) );
			TypeList.Add( typeof( QuarterStaff ), typeof( GlassStaff ) );
			TypeList.Add( typeof( GnarledStaff ), typeof( SerpentStoneStaff ) );
			TypeList.Add( typeof( OrnateAxe ), typeof( DualShortAxes ) );
			TypeList.Add( typeof( DoubleAxe ), typeof( DualShortAxes ) );
			TypeList.Add( typeof( ExecutionersAxe ), typeof( DualShortAxes ) );
			TypeList.Add( typeof( Axe ), typeof( DualShortAxes ) );
			TypeList.Add( typeof( TwoHandedAxe ), typeof( DualShortAxes ) );
			TypeList.Add( typeof( LargeBattleAxe ), typeof( DualShortAxes ) );
			TypeList.Add( typeof( Spear ), typeof( DualPointedSpear ) );
			TypeList.Add( typeof( Kama ), typeof( DualPointedSpear ) );
			TypeList.Add( typeof( Sai ), typeof( DualPointedSpear ) );
			TypeList.Add( typeof( ShortSpear ), typeof( DualPointedSpear ) );
			TypeList.Add( typeof( BladedStaff ), typeof( DualPointedSpear ) );
			TypeList.Add( typeof( DoubleBladedStaff ), typeof( DualPointedSpear ) );
			TypeList.Add( typeof( Dagger ), typeof( GargishDagger ) );
			TypeList.Add( typeof( SkinningKnife ), typeof( GargishDagger ) );

			// Shields
			TypeList.Add( typeof( MetalKiteShield ), typeof( GargishKiteShield ) );
			TypeList.Add( typeof( WoodenKiteShield ), typeof( LargeStoneShield ) );
			TypeList.Add( typeof( Buckler ), typeof( SmallPlateShield ) );
			TypeList.Add( typeof( BronzeShield ), typeof( SmallPlateShield ) );
			TypeList.Add( typeof( HeaterShield ), typeof( LargePlateShield ) );
			TypeList.Add( typeof( WoodenShield ), typeof( GargishWoodenShield ) );
			TypeList.Add( typeof( MetalShield ), typeof( MediumPlateShield ) );
			TypeList.Add( typeof( ChaosShield ), typeof( GargishChaosShield ) );
			TypeList.Add( typeof( OrderShield ), typeof( GargishOrderShield ) );

			// Armor
			// Due to Male/Female versions the Dictionary cannot be done like the rest are listed.

		}

		public static bool TryToConvert( Mobile from, Item olditem )
		{
			if ( !TypeList.ContainsKey( olditem.GetType() ) )
				return false;

			Item newitem = CreateItem( TypeList[olditem.GetType()] );

			if ( newitem == null )
				return false;

			if ( olditem is BaseWeapon && newitem is BaseWeapon )
			{
				BaseWeapon oldweapon = (BaseWeapon)olditem;
				BaseWeapon newweapon = (BaseWeapon)newitem;

				newweapon.Attributes = new AosAttributes( oldweapon, newweapon.Attributes );
				//newweapon.ElementDamages = new AosElementAttributes( oldweapon, newweapon.ElementDamages );
				newweapon.SkillBonuses = new AosSkillBonuses( oldweapon, newweapon.SkillBonuses );
				newweapon.WeaponAttributes = new AosWeaponAttributes( oldweapon, newweapon.WeaponAttributes );
				newweapon.AbsorptionAttributes = new SAAbsorptionAttributes( oldweapon, newweapon.AbsorptionAttributes );
			}
			else if ( olditem is BaseArmor && newitem is BaseArmor )
			{
				BaseArmor oldarmor = (BaseArmor)olditem;
				BaseArmor newarmor = (BaseArmor)newitem;

				newarmor.Attributes = new AosAttributes( oldarmor, newarmor.Attributes );
				newarmor.ArmorAttributes = new AosArmorAttributes( oldarmor, newarmor.ArmorAttributes );
				newarmor.SkillBonuses = new AosSkillBonuses( oldarmor, newarmor.SkillBonuses );
				newarmor.AbsorptionAttributes = new SAAbsorptionAttributes( oldarmor, newarmor.AbsorptionAttributes );
			}
			else
			{
				return false;
			}

			olditem.Delete();

			if ( from.Backpack == null )
				newitem.MoveToWorld( from.Location, from.Map );
			else
				from.Backpack.DropItem( newitem );

			return true;
		}


		public static Item CreateItem( Type t )
		{
			try
			{
				return (Item)Activator.CreateInstance( t );
			}
			catch
			{
				return null;
			}
		}

	}
}

namespace Server.Commands
{
	public class GarishConversionCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "GargishConvert", AccessLevel.GameMaster, new CommandEventHandler( GargishConvert_OnCommand ) );
		}

		[Description( "Converts a human/elf item into a a gargoyle item." )]
		public static void GargishConvert_OnCommand( CommandEventArgs e )
		{
			e.Mobile.BeginTarget( 10, false, TargetFlags.None, new TargetCallback( GargishConvert_CallBack ) );
		}

		public static void GargishConvert_CallBack( Mobile from, object targeted )
		{
			if ( targeted is Item )
			{
				if ( GargishConverter.TryToConvert( from, (Item)targeted ) )
					from.SendMessage( "The item has been turned into a gargish item." );
				else
					from.SendMessage( "That could not be converted." );
			}
			else
				from.SendMessage("That is not an item.");
		}
	}
}
/*



*/
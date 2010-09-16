/***************************************************************************
 *                          ExpansionInfo.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: ExpansionInfo.cs 187 2007-05-26 03:12:41Z asayre $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;

namespace Server
{
	public enum Expansion
	{
		None,
		/*
		T2A,
		UOR,
		LBR,
		UOTD,
		*/
		AOS,
		SE,
		ML,
		SA
	}
	public class ExpansionInfo
	{
		private string m_Name;
		private int m_ID, m_NetStateFlag, m_SupportedFeatures, m_CharListFlags, m_CustomHousingFlag;

		private ClientVersion m_RequiredClient;	//Used as an alternative to the flags

		public string Name{ get{ return m_Name; } }
		public int ID{ get{ return m_ID; } }
		public int NetStateFlag{ get{ return m_NetStateFlag; } }
		public int SupportedFeatures{ get{ return m_SupportedFeatures; } }
		public int CharacterListFlags { get { return m_CharListFlags; } }
		public int CustomHousingFlag { get{ return m_CustomHousingFlag; } }
		public ClientVersion RequiredClient { get { return m_RequiredClient; } }

		public ExpansionInfo( int id, string name, int netStateFlag, int supportedFeatures, int charListFlags, int customHousingFlag )
		{
			m_Name = name;
			m_ID = id;
			m_NetStateFlag = netStateFlag;
			m_SupportedFeatures = supportedFeatures;
			m_CharListFlags = charListFlags;
			m_CustomHousingFlag = customHousingFlag;
		}

		public ExpansionInfo( int id, string name, ClientVersion requiredClient, int supportedFeatures, int charListFlags, int customHousingFlag )
		{
			m_Name = name;
			m_ID = id;
			m_SupportedFeatures = supportedFeatures;
			m_CharListFlags = charListFlags;
			m_CustomHousingFlag = customHousingFlag;
			m_RequiredClient = requiredClient;
		}

		public static ExpansionInfo[] Table { get { return m_Table; } }
		private static ExpansionInfo[] m_Table = new ExpansionInfo[]
			{
				new ExpansionInfo( 0, "None"			, 0x00,								0x0003, 0x008, 0x00 ),
				new ExpansionInfo( 1, "Age of Shadows"	, 0x08,								0x801F, 0x028, 0x20 ),
				new ExpansionInfo( 2, "Samurai Empire"	, 0x10,								0x805F, 0x0A8, 0x60 ),	//0x40 | 0x20 = 0x60
				new ExpansionInfo( 3, "Mondain's Legacy", new ClientVersion( "5.0.0a" ),	0x82DF, 0x1A8, 0x2E0 ),	//0x280 | 0x60 = 0x2E0
				#region SA
				new ExpansionInfo( 4, "Stygian Abyss",	new ClientVersion( "7.0.0" ),		0x182DF, 0x11A8, 0x2E0 )
				#endregion
				//0x200 + 0x400 for KR?

/*
Supported Features
0x01 = T2A: adds the chat button and regions.
0x02 = Renaissance expansion (dunno what it adds).
0x04 = Third Dawn expansion.
0x08 = LBR expansion: some skills, Ilshenar's map.
0x10 = AOS expansion: necro/chiv skills & spells, Malas map, Book of Arms in the paper doll.
0x20 = Allow a 6th character to be made.
0x40 = SE expansion: bushido/ninjitsu spells & skills, Tokuno map.
0x80 = ML features: elf race, Spellweaving spells & skill.
0x100 = Eight Age splash screen.
0x200 = Ninth Age splash screen.
0x400 = Tenth Age splash screen.
0x800 = Enable increased housing and bank storage.
0x1000 = Allow a 7th character to be made.
0x2000 = Enable roleplay faces (KR only?).
0x4000 = Trial Account flag.
0x8000 = Eleventh Age splash screen.
0x10000 = SA expansion: gargoyle race, imbuing spells, three new skills.
T2A + Ren + 3rd + LBR + AOS + SE + ML + 9th screen + 11th age + SA = 182DF

Char List
0x01 = Unknown.
0x02 = Overwrite configuration button.
0x04 = Limit 1 character per account (must use 0x10 flag as well).
0x08 = Allow context menus.
0x10 = Limit character slots (must use 0x04 flag as well).
0x20 = Allow paladin and necromancer professions to be choosen.
0x40 = 6th character slot.
0x80 = Allow samurai and ninja professions to be choosen.
0x100 = Elven race.
0x200 = Unknown
0x400 = Send UO3D client support.
0x800 = Unknown.
0x1000 = 7th character slot.
0x2000 = unknown.
Context Menus + AOS Classes + SE Classes + Elves + 7th slot = 0x11A8

Housing
0x20 = Enable AOS housing tiles (a must for custom housing).
0x40 = Enable SE housing tiles.
0x280 = Enable ML housing tiles.
SA tiles work under the ML flag.
AOS + SE + ML + SA = 0x2E0
*/

				//0x200 + 0x400 for KR?
			};

		public static ExpansionInfo GetInfo( Expansion ex )
		{
			return GetInfo( (int)ex );
		}

		public static ExpansionInfo GetInfo( int ex )
		{
			int v = (int)ex;

			if( v < 0 || v >= m_Table.Length )
				v = 0;

			return m_Table[v];
		}

		public static ExpansionInfo CurrentExpansion { get { return GetInfo( Core.Expansion ); } }

		public override string ToString()
		{
			return m_Name;
		}
	}
}

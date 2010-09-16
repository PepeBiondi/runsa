/***************************************************************************
 *                               MultiData.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: MultiData.cs 4 2006-06-15 04:28:39Z mark $
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
using System.Collections;
using System.IO;

namespace Server
{
	public class MultiData
	{
		private static MultiComponentList[] m_Components;

		private static FileStream m_Index, m_Stream;
		private static BinaryReader m_IndexReader, m_StreamReader;

		public static MultiComponentList GetComponents( int multiID )
		{
			MultiComponentList mcl;

			multiID &= 0x3FFF;

			if ( multiID >= 0 && multiID < m_Components.Length )
			{
				mcl = m_Components[multiID];

				if ( mcl == null )
					m_Components[multiID] = mcl = Load( multiID );
			}
			else
			{
				mcl = MultiComponentList.Empty;
			}

			return mcl;
		}

		public static MultiComponentList Load( int multiID )
		{
			try
			{
				m_IndexReader.BaseStream.Seek( multiID * 12, SeekOrigin.Begin );

				int lookup = m_IndexReader.ReadInt32();
				int length = m_IndexReader.ReadInt32();

				if ( lookup < 0 || length <= 0 )
					return MultiComponentList.Empty;

				m_StreamReader.BaseStream.Seek( lookup, SeekOrigin.Begin );

				return new MultiComponentList( m_StreamReader, length / 12 );
			}
			catch
			{
				return MultiComponentList.Empty;
			}
		}

		static MultiData()
		{
			string idxPath = Core.FindDataFile( "multi.idx" );
			string mulPath = Core.FindDataFile( "multi.mul" );

			if ( File.Exists( idxPath ) && File.Exists( mulPath ) )
			{
				m_Index = new FileStream( idxPath, FileMode.Open, FileAccess.Read, FileShare.Read );
				m_IndexReader = new BinaryReader( m_Index );

				m_Stream = new FileStream( mulPath, FileMode.Open, FileAccess.Read, FileShare.Read );
				m_StreamReader = new BinaryReader( m_Stream );

				m_Components = new MultiComponentList[(int)(m_Index.Length / 12)];

				string vdPath = Core.FindDataFile( "verdata.mul" );

				if ( File.Exists( vdPath ) )
				{
					using ( FileStream fs = new FileStream( vdPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
					{
						BinaryReader bin = new BinaryReader( fs );

						int count = bin.ReadInt32();

						for ( int i = 0; i < count; ++i )
						{
							int file = bin.ReadInt32();
							int index = bin.ReadInt32();
							int lookup = bin.ReadInt32();
							int length = bin.ReadInt32();
							int extra = bin.ReadInt32();

							if ( file == 14 && index >= 0 && index < m_Components.Length && lookup >= 0 && length > 0 )
							{
								bin.BaseStream.Seek( lookup, SeekOrigin.Begin );

								m_Components[index] = new MultiComponentList( bin, length / 12 );

								bin.BaseStream.Seek( 24 + (i * 20), SeekOrigin.Begin );
							}
						}

						bin.Close();
					}
				}
			}
			else
			{
				Console.WriteLine( "Warning: Multi data files not found" );

				m_Components = new MultiComponentList[0];
			}
		}
	}

	public struct MultiTileEntry
	{
		public short m_ItemID;
		public short m_OffsetX, m_OffsetY, m_OffsetZ;
		public int m_Flags;

		public MultiTileEntry( short itemID, short xOffset, short yOffset, short zOffset, int flags )
		{
			m_ItemID = itemID;
			m_OffsetX = xOffset;
			m_OffsetY = yOffset;
			m_OffsetZ = zOffset;
			m_Flags = flags;
		}
	}

	public sealed class MultiComponentList
	{
		private Point2D m_Min, m_Max, m_Center;
		private int m_Width, m_Height;
		private Tile[][][] m_Tiles;
		private MultiTileEntry[] m_List;

		public static readonly MultiComponentList Empty = new MultiComponentList();

		public Point2D Min{ get{ return m_Min; } }
		public Point2D Max{ get{ return m_Max; } }

		public Point2D Center{ get{ return m_Center; } }

		public int Width{ get{ return m_Width; } }
		public int Height{ get{ return m_Height; } }

		public Tile[][][] Tiles{ get{ return m_Tiles; } }
		public MultiTileEntry[] List{ get{ return m_List; } }

		public void Add( int itemID, int x, int y, int z )
		{
			#region SA
			itemID &= 0x7FFF;
			itemID |= 0x8000; // Note #2 8000 prevented you from walking on them
			// 4003 | 8000 = C003 & 7FFF = 4003
			// 4003 | 4000 = 3 & 7FFF = 3
			itemID &= 0x7FFF;
			#endregion

			int vx = x + m_Center.m_X;
			int vy = y + m_Center.m_Y;

			if ( vx >= 0 && vx < m_Width && vy >= 0 && vy < m_Height )
			{
				Tile[] oldTiles = m_Tiles[vx][vy];

				for ( int i = oldTiles.Length - 1; i >= 0; --i )
				{
					#region SA
					ItemData data = TileData.ItemTable[itemID & 0x7FFF];
					#endregion

					if ( oldTiles[i].Z == z && (oldTiles[i].Height > 0 == data.Height > 0 ) )
					{
						bool newIsRoof = ( data.Flags & TileFlag.Roof) != 0;
						#region SA
						bool oldIsRoof = (TileData.ItemTable[oldTiles[i].ID & 0x7FFF].Flags & TileFlag.Roof ) != 0;
						#endregion

						if ( newIsRoof == oldIsRoof )
							Remove( oldTiles[i].ID, x, y, z );
					}
				}

				oldTiles = m_Tiles[vx][vy];

				Tile[] newTiles = new Tile[oldTiles.Length + 1];

				for ( int i = 0; i < oldTiles.Length; ++i )
					newTiles[i] = oldTiles[i];

				newTiles[oldTiles.Length] = new Tile( (short)itemID, (sbyte)z );

				m_Tiles[vx][vy] = newTiles;

				MultiTileEntry[] oldList = m_List;
				MultiTileEntry[] newList = new MultiTileEntry[oldList.Length + 1];

				for ( int i = 0; i < oldList.Length; ++i )
					newList[i] = oldList[i];

				newList[oldList.Length] = new MultiTileEntry( (short)itemID, (short)x, (short)y, (short)z, 1 );

				m_List = newList;

				if ( x < m_Min.m_X )
					m_Min.m_X = x;

				if ( y < m_Min.m_Y )
					m_Min.m_Y = y;

				if ( x > m_Max.m_X )
					m_Max.m_X = x;

				if ( y > m_Max.m_Y )
					m_Max.m_Y = y;
			}
		}

		public void RemoveXYZH( int x, int y, int z, int minHeight )
		{
			int vx = x + m_Center.m_X;
			int vy = y + m_Center.m_Y;

			if ( vx >= 0 && vx < m_Width && vy >= 0 && vy < m_Height )
			{
				Tile[] oldTiles = m_Tiles[vx][vy];

				for ( int i = 0; i < oldTiles.Length; ++i )
				{
					Tile tile = oldTiles[i];

					if ( tile.Z == z && tile.Height >= minHeight )
					{
						Tile[] newTiles = new Tile[oldTiles.Length - 1];

						for ( int j = 0; j < i; ++j )
							newTiles[j] = oldTiles[j];

						for ( int j = i + 1; j < oldTiles.Length; ++j )
							newTiles[j - 1] = oldTiles[j];

						m_Tiles[vx][vy] = newTiles;

						break;
					}
				}

				MultiTileEntry[] oldList = m_List;

				for ( int i = 0; i < oldList.Length; ++i )
				{
					MultiTileEntry tile = oldList[i];

					#region SA
					if ( tile.m_OffsetX == (short)x && tile.m_OffsetY == (short)y && tile.m_OffsetZ == (short)z && TileData.ItemTable[tile.m_ItemID & 0x7FFF].Height >= minHeight )
					{
					#endregion
						MultiTileEntry[] newList = new MultiTileEntry[oldList.Length - 1];

						for ( int j = 0; j < i; ++j )
							newList[j] = oldList[j];

						for ( int j = i + 1; j < oldList.Length; ++j )
							newList[j - 1] = oldList[j];

						m_List = newList;

						break;
					}
				}
			}
		}

		public void Remove( int itemID, int x, int y, int z )
		{
			int vx = x + m_Center.m_X;
			int vy = y + m_Center.m_Y;

			if ( vx >= 0 && vx < m_Width && vy >= 0 && vy < m_Height )
			{
				Tile[] oldTiles = m_Tiles[vx][vy];

				for ( int i = 0; i < oldTiles.Length; ++i )
				{
					Tile tile = oldTiles[i];

					#region SA
					if ( (tile.ID & 0x7FFF) == (itemID & 0x7FFF) && tile.Z == z )
					{
					#endregion

						Tile[] newTiles = new Tile[oldTiles.Length - 1];

						for ( int j = 0; j < i; ++j )
							newTiles[j] = oldTiles[j];

						for ( int j = i + 1; j < oldTiles.Length; ++j )
							newTiles[j - 1] = oldTiles[j];

						m_Tiles[vx][vy] = newTiles;

						break;
					}
				}

				MultiTileEntry[] oldList = m_List;

				for ( int i = 0; i < oldList.Length; ++i )
				{
					MultiTileEntry tile = oldList[i];

					#region SA
					if ( (tile.m_ItemID & 0x7FFF) == (short)(itemID & 0x7FFF) && tile.m_OffsetX == (short)x && tile.m_OffsetY == (short)y && tile.m_OffsetZ == (short)z )
					{
					#endregion

						MultiTileEntry[] newList = new MultiTileEntry[oldList.Length - 1];

						for ( int j = 0; j < i; ++j )
							newList[j] = oldList[j];

						for ( int j = i + 1; j < oldList.Length; ++j )
							newList[j - 1] = oldList[j];

						m_List = newList;

						break;
					}
				}
			}
		}

		public void Resize( int newWidth, int newHeight )
		{
			int oldWidth = m_Width, oldHeight = m_Height;
			Tile[][][] oldTiles = m_Tiles;

			int totalLength = 0;

			Tile[][][] newTiles = new Tile[newWidth][][];

			for ( int x = 0; x < newWidth; ++x )
			{
				newTiles[x] = new Tile[newHeight][];

				for ( int y = 0; y < newHeight; ++y )
				{
					if ( x < oldWidth && y < oldHeight )
						newTiles[x][y] = oldTiles[x][y];
					else
						newTiles[x][y] = new Tile[0];

					totalLength += newTiles[x][y].Length;
				}
			}

			m_Tiles = newTiles;
			m_List = new MultiTileEntry[totalLength];
			m_Width = newWidth;
			m_Height = newHeight;

			m_Min = Point2D.Zero;
			m_Max = Point2D.Zero;

			int index = 0;

			for ( int x = 0; x < newWidth; ++x )
			{
				for ( int y = 0; y < newHeight; ++y )
				{
					Tile[] tiles = newTiles[x][y];

					for ( int i = 0; i < tiles.Length; ++i )
					{
						Tile tile = tiles[i];

						int vx = x - m_Center.X;
						int vy = y - m_Center.Y;

						if ( vx < m_Min.m_X )
							m_Min.m_X = vx;

						if ( vy < m_Min.m_Y )
							m_Min.m_Y = vy;

						if ( vx > m_Max.m_X )
							m_Max.m_X = vx;

						if ( vy > m_Max.m_Y )
							m_Max.m_Y = vy;

						m_List[index++] = new MultiTileEntry( (short)tile.ID, (short)vx, (short)vy, (short)tile.Z, 1 );
					}
				}
			}
		}

		public MultiComponentList( MultiComponentList toCopy )
		{
			m_Min = toCopy.m_Min;
			m_Max = toCopy.m_Max;

			m_Center = toCopy.m_Center;

			m_Width = toCopy.m_Width;
			m_Height = toCopy.m_Height;

			m_Tiles = new Tile[m_Width][][];

			for ( int x = 0; x < m_Width; ++x )
			{
				m_Tiles[x] = new Tile[m_Height][];

				for ( int y = 0; y < m_Height; ++y )
				{
					m_Tiles[x][y] = new Tile[toCopy.m_Tiles[x][y].Length];

					for ( int i = 0; i < m_Tiles[x][y].Length; ++i )
						m_Tiles[x][y][i] = toCopy.m_Tiles[x][y][i];
				}
			}

			m_List = new MultiTileEntry[toCopy.m_List.Length];

			for ( int i = 0; i < m_List.Length; ++i )
				m_List[i] = toCopy.m_List[i];
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( (int) 0 ); // version;

			writer.Write( m_Min );
			writer.Write( m_Max );
			writer.Write( m_Center );

			writer.Write( (int) m_Width );
			writer.Write( (int) m_Height );

			writer.Write( (int) m_List.Length );

			for ( int i = 0; i < m_List.Length; ++i )
			{
				MultiTileEntry ent = m_List[i];

				writer.Write( (short) ent.m_ItemID );
				writer.Write( (short) ent.m_OffsetX );
				writer.Write( (short) ent.m_OffsetY );
				writer.Write( (short) ent.m_OffsetZ );
				writer.Write( (int) ent.m_Flags );
			}
		}

		public MultiComponentList( GenericReader reader )
		{
			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Min = reader.ReadPoint2D();
					m_Max = reader.ReadPoint2D();
					m_Center = reader.ReadPoint2D();
					m_Width = reader.ReadInt();
					m_Height = reader.ReadInt();

					int length = reader.ReadInt();

					MultiTileEntry[] allTiles = m_List = new MultiTileEntry[length];

					for ( int i = 0; i < length; ++i )
					{
						allTiles[i].m_ItemID = reader.ReadShort();
						allTiles[i].m_OffsetX = reader.ReadShort();
						allTiles[i].m_OffsetY = reader.ReadShort();
						allTiles[i].m_OffsetZ = reader.ReadShort();
						allTiles[i].m_Flags = reader.ReadInt();
					}

					TileList[][] tiles = new TileList[m_Width][];
					m_Tiles = new Tile[m_Width][][];

					for ( int x = 0; x < m_Width; ++x )
					{
						tiles[x] = new TileList[m_Height];
						m_Tiles[x] = new Tile[m_Height][];

						for ( int y = 0; y < m_Height; ++y )
							tiles[x][y] = new TileList();
					}

					for ( int i = 0; i < allTiles.Length; ++i )
					{
						if ( i == 0 || allTiles[i].m_Flags != 0 )
						{
							int xOffset = allTiles[i].m_OffsetX + m_Center.m_X;
							int yOffset = allTiles[i].m_OffsetY + m_Center.m_Y;

							#region SA
							// Note #1 | 0x8000 produces problems, namely the server doesn't increase your Z when climbing stairs thus the itemid isn't reconized properly.
							tiles[xOffset][yOffset].Add( (short)((allTiles[i].m_ItemID & 0x7FFF) | 0x4000), (sbyte)allTiles[i].m_OffsetZ );
							#endregion
						}
					}

					for ( int x = 0; x < m_Width; ++x )
						for ( int y = 0; y < m_Height; ++y )
							m_Tiles[x][y] = tiles[x][y].ToArray();

					break;
				}
			}
		}

		public MultiComponentList( BinaryReader reader, int count )
		{
			MultiTileEntry[] allTiles = m_List = new MultiTileEntry[count];

			for ( int i = 0; i < count; ++i )
			{
				allTiles[i].m_ItemID = reader.ReadInt16();
				allTiles[i].m_OffsetX = reader.ReadInt16();
				allTiles[i].m_OffsetY = reader.ReadInt16();
				allTiles[i].m_OffsetZ = reader.ReadInt16();
				allTiles[i].m_Flags = reader.ReadInt32();

				MultiTileEntry e = allTiles[i];

				if ( i == 0 || e.m_Flags != 0 )
				{
					if ( e.m_OffsetX < m_Min.m_X )
						m_Min.m_X = e.m_OffsetX;

					if ( e.m_OffsetY < m_Min.m_Y )
						m_Min.m_Y = e.m_OffsetY;

					if ( e.m_OffsetX > m_Max.m_X )
						m_Max.m_X = e.m_OffsetX;

					if ( e.m_OffsetY > m_Max.m_Y )
						m_Max.m_Y = e.m_OffsetY;
				}
			}

			m_Center = new Point2D( -m_Min.m_X, -m_Min.m_Y );
			m_Width = (m_Max.m_X - m_Min.m_X) + 1;
			m_Height = (m_Max.m_Y - m_Min.m_Y) + 1;

			TileList[][] tiles = new TileList[m_Width][];
			m_Tiles = new Tile[m_Width][][];

			for ( int x = 0; x < m_Width; ++x )
			{
				tiles[x] = new TileList[m_Height];
				m_Tiles[x] = new Tile[m_Height][];

				for ( int y = 0; y < m_Height; ++y )
					tiles[x][y] = new TileList();
			}

			for ( int i = 0; i < allTiles.Length; ++i )
			{
				if ( i == 0 || allTiles[i].m_Flags != 0 )
				{
					int xOffset = allTiles[i].m_OffsetX + m_Center.m_X;
					int yOffset = allTiles[i].m_OffsetY + m_Center.m_Y;

					#region SA
					// Note #1 | 0x8000 produces problems, namely the server doesn't increase your Z when climbing stairs thus the itemid isn't reconized properly.
					tiles[xOffset][yOffset].Add( (short)((allTiles[i].m_ItemID & 0x7FFF) | 0x4000), (sbyte)allTiles[i].m_OffsetZ );
					#endregion
				}
			}

			for ( int x = 0; x < m_Width; ++x )
				for ( int y = 0; y < m_Height; ++y )
					m_Tiles[x][y] = tiles[x][y].ToArray();
		}

		private MultiComponentList()
		{
			m_Tiles = new Tile[0][][];
			m_List = new MultiTileEntry[0];
		}
	}
}
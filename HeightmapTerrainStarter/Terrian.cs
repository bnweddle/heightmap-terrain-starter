﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeightmapTerrainStarter
{
    /// <summary>
    /// A class representing terrain
    /// </summary>
    public class Terrain
    {
        // The game this Terrain belongs to
        Game game;

        // The height data 
        float[,] heights;

        // The number of cells in the x-axis
        int width;

        // The number of cells in the z-axis
        int height;

        // The number of triangles in the mesh
        int triangles;

        // The terrain mesh vertices
        VertexBuffer vertices;

        // The terrain mesh indices
        IndexBuffer indices;

        // The effect used to render the terrain
        BasicEffect effect;

        // The texture to apply to the terrain surface
        Texture2D texture;

        /// <summary>
        /// Constructs a new Terrain
        /// </summary>
        /// <param name="game">The game this Terrain belongs to</param>
        /// <param name="heightmap">The hieghtmap used to set heihgts</param>
        /// <param name="heightRange">The difference between the lowest and highest elevation in the terrain</param>
        /// <param name="world">The terrain's positon and orientation in the world</param>
        public Terrain(Game game, Texture2D heightmap, float heightRange, Matrix world)
        {
            this.game = game;
            texture = game.Content.Load<Texture2D>("ground_grass_gen_08");
            LoadHeights(heightmap, heightRange);
            InitializeVertices();
            InitializeIndices();
            InitializeEffect(world);
        }


        /// <summary>
        /// Converts the supplied Texture2D into height data
        /// </summary>
        /// <param name="heightmap">The heightmap texture</param>
        /// <param name="scale">The difference between the highest and lowest elevation</param>
        private void LoadHeights(Texture2D heightmap, float scale)
        {
            // Convert the scale factor to work with our color
            scale /= 256;
            // The number of grid cells in the x-direction
            width = heightmap.Width;

            // The number of grid cells in the z-direction
            height = heightmap.Height;
            heights = new float[width, height];
            // Get the color data from the heightmap
            Color[] heightmapColors = new Color[width * height];
            heightmap.GetData<Color>(heightmapColors);

            // Set the heights
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    heights[x, y] = heightmapColors[x + y * width].R * scale;
                }
            }

        }

        /// <summary>
        /// Creates the terrain vertex buffer
        /// </summary>
        private void InitializeVertices()
        {
            VertexPositionNormalTexture[] terrainVertices = new VertexPositionNormalTexture[width * height];
            int i = 0;
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    terrainVertices[i].Position = new Vector3(x, heights[x, z], -z);
                    terrainVertices[i].Normal = Vector3.Up;
                    terrainVertices[i].TextureCoordinate = new Vector2((float)x / 50f, (float)z / 50f);
                    i++;
                }
            }
            vertices = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalTexture), terrainVertices.Length, BufferUsage.None);
            vertices.SetData<VertexPositionNormalTexture>(terrainVertices);
        }

        /// <summary>
        /// Creates the index buffer
        /// </summary>
        private void InitializeIndices()
        {
            // The number of triangles in the triangle strip
            triangles = (width) * 2 * (height - 1);

            int[] terrainIndices = new int[triangles];

            int i = 0;
            int z = 0;
            while (z < height - 1)
            {
                for (int x = 0; x < width; x++)
                {
                    terrainIndices[i++] = x + z * width;
                    terrainIndices[i++] = x + (z + 1) * width;
                }
                z++;
                if (z < height - 1)
                {
                    for (int x = width - 1; x >= 0; x--)
                    {
                        terrainIndices[i++] = x + (z + 1) * width;
                        terrainIndices[i++] = x + z * width;
                    }
                }
                z++;
            }

            IndexElementSize elementSize = (width * height > short.MaxValue) ? IndexElementSize.ThirtyTwoBits : IndexElementSize.SixteenBits;

            indices = new IndexBuffer(game.GraphicsDevice, elementSize, terrainIndices.Length, BufferUsage.None);
            indices.SetData<int>(terrainIndices);
        }

        /// <summary>
        /// Initialize the effect used to render the terrain
        /// </summary>
        /// <param name="world">The world matrix</param>
        private void InitializeEffect(Matrix world)
        {
            effect = new BasicEffect(game.GraphicsDevice);
            effect.World = world;
            effect.Texture = texture;
            effect.TextureEnabled = true;
        }

        /// <summary>
        /// Draws the terrain
        /// </summary>
        /// <param name="camera">The camera to use</param>
        public void Draw(ICamera camera)
        {
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.Indices = indices;
            game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, triangles);
        }




    }
}

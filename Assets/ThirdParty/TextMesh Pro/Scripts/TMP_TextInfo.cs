// Copyright (C) 2014 - 2016 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace TMPro
{
    /// <summary>
    /// Class which contains information about every element contained within the text object.
    /// </summary>
    [Serializable]
    public class TMP_TextInfo
    {
        private static Vector2 k_InfinityVectorPositive = new Vector2(1000000, 1000000);
        private static Vector2 k_InfinityVectorNegative = new Vector2(-1000000, -1000000);

        public TMP_Text textComponent;

        public int characterCount;
        public int spriteCount;
        public int spaceCount;
        public int wordCount;
        public int linkCount;
        public int lineCount;
        public int pageCount;

        public int materialCount;

        public TMP_CharacterInfo[] characterInfo;
        public TMP_WordInfo[] wordInfo;
        public TMP_LinkInfo[] linkInfo;
        public TMP_LineInfo[] lineInfo;
        public TMP_PageInfo[] pageInfo;
        public TMP_MeshInfo[] meshInfo;


        // Default Constructor
        public TMP_TextInfo()
        {
            characterInfo = new TMP_CharacterInfo[8];
            wordInfo = new TMP_WordInfo[16];
            linkInfo = new TMP_LinkInfo[0];
            lineInfo = new TMP_LineInfo[2];
            pageInfo = new TMP_PageInfo[16];

            meshInfo = new TMP_MeshInfo[1];
        }


        public TMP_TextInfo(TMP_Text textComponent)
        {
            this.textComponent = textComponent;

            characterInfo = new TMP_CharacterInfo[8];

            wordInfo = new TMP_WordInfo[4];
            linkInfo = new TMP_LinkInfo[0];

            lineInfo = new TMP_LineInfo[2];
            pageInfo = new TMP_PageInfo[16];

            meshInfo = new TMP_MeshInfo[1];
            meshInfo[0].mesh = textComponent.mesh;
            materialCount = 1;
        }


        /// <summary>
        /// Function to clear the counters of the text object.
        /// </summary>
        public void Clear()
        {
            characterCount = 0;
            spaceCount = 0;
            wordCount = 0;
            linkCount = 0;
            lineCount = 0;
            pageCount = 0;
            spriteCount = 0;

            for (int i = 0; i < this.meshInfo.Length; i++)
            {
                this.meshInfo[i].vertexCount = 0;
            }
        }


        /// <summary>
        /// Function to clear the content of the MeshInfo array while preserving the Triangles, Normals and Tangents.
        /// </summary>
        public void ClearMeshInfo(bool updateMesh)
        {
            for (int i = 0; i < this.meshInfo.Length; i++)
                this.meshInfo[i].Clear(updateMesh);
        }


        /// <summary>
        /// Function to clear the content of all the MeshInfo arrays while preserving their Triangles, Normals and Tangents.
        /// </summary>
        public void ClearAllMeshInfo()
        {
            for (int i = 0; i < this.meshInfo.Length; i++)
                this.meshInfo[i].Clear(true);
        }


        /// <summary>
        /// Function used to mark unused vertices as degenerate.
        /// </summary>
        /// <param name="materials"></param>
        public void ClearUnusedVertices(MaterialReference[] materials)
        {
            for (int i = 0; i < meshInfo.Length; i++)
            {
                int start = 0; // materials[i].referenceCount * 4;
                meshInfo[i].ClearUnusedVertices(start);
            }
        }


        /// <summary>
        /// Function to clear and initialize the lineInfo array. 
        /// </summary>
        public void ClearLineInfo()
        {
            if (this.lineInfo == null)
                this.lineInfo = new TMP_LineInfo[2];

            for (int i = 0; i < this.lineInfo.Length; i++)
            {
                this.lineInfo[i].characterCount = 0;
                this.lineInfo[i].spaceCount = 0;
                //this.lineInfo[i].wordCount = 0;
                this.lineInfo[i].width = 0;

                this.lineInfo[i].ascender = k_InfinityVectorNegative.x;
                this.lineInfo[i].descender = k_InfinityVectorPositive.x;

                this.lineInfo[i].lineExtents.min = k_InfinityVectorPositive;
                this.lineInfo[i].lineExtents.max = k_InfinityVectorNegative;

                this.lineInfo[i].maxAdvance = 0;
                //this.lineInfo[i].maxScale = 0;

            }
        }


        /// <summary>
        /// Function to resize any of the structure contained in the TMP_TextInfo class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="size"></param>
        public static void Resize<T> (ref T[] array, int size)
        {
            // Allocated to the next power of two
            int newSize = size > 1024 ? size + 256 : Mathf.NextPowerOfTwo(size);

            Array.Resize(ref array, newSize);
        }


        /// <summary>
        /// Function to resize any of the structure contained in the TMP_TextInfo class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="size"></param>
        /// <param name="isFixedSize"></param>
        public static void Resize<T>(ref T[] array, int size, bool isBlockAllocated)
        {
            //if (size <= array.Length) return;

            if (isBlockAllocated) size = size > 1024 ? size + 256 : Mathf.NextPowerOfTwo(size);

            if (size == array.Length) return;

            Array.Resize(ref array, size);
        }

    }
}

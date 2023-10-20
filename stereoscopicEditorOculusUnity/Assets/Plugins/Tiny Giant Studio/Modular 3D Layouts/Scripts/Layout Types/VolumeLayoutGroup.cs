﻿using System;
using System.Collections.Generic;
using UnityEngine;
#if MODULAR_3D_TEXT
using TinyGiantStudio.Text;
#endif

namespace TinyGiantStudio.Layout
{
    //[HelpURL("https://ferdowsur.gitbook.io/layout-system/layout-group/grid-layout-group")]
    [AddComponentMenu("Tiny Giant Studio/Layout/Volume Layout Group (M3D)", 30001)]
    public class VolumeLayoutGroup : LayoutGroup
    {
        #region Variable declaration

        [SerializeField]
        private Alignment _anchor = Alignment.MiddleCenter;
        public Alignment Anchor
        {
            get { return _anchor; }
            set
            {
                _anchor = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [SerializeField]
        private DepthAlignment _depthAlignment = DepthAlignment.middle;
        public DepthAlignment DepthAlignment
        {
            get { return _depthAlignment; }
            set
            {
                _depthAlignment = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [Tooltip("Elements will be split to fill the entire width")]
        [SerializeField]
        private bool _justiceHorizontal = false;
        public bool JusticeHorizontal
        {
            get { return _justiceHorizontal; }
            set
            {
                _justiceHorizontal = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [Tooltip("Justice will be only be applied if the elements hold equal/more than the % width")]
        [SerializeField]
        private float _justiceHorizontalPercent = 70;
        public float JusticeHorizontalPercent
        {
            get { return _justiceHorizontalPercent; }
            set
            {
                _justiceHorizontalPercent = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [Tooltip("Elements will be split to fill the entire width")]
        [SerializeField]
        private bool _justiceVertical = false;
        public bool JusticeVertical
        {
            get { return _justiceVertical; }
            set
            {
                _justiceVertical = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [Tooltip("Justice will be only be applied if the elements hold equal/more than the % height")]
        [SerializeField]
        private float _justiceVerticalPercent = 70;
        public float JusticeVerticalPercent
        {
            get { return _justiceVerticalPercent; }
            set
            {
                _justiceVerticalPercent = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [SerializeField]
        private Vector3 _spacing = new Vector3(5, 5, 5);
        /// <summary>
        /// This will always return _spacing/100
        /// </summary>
        public Vector3 Spacing
        {
            get { return _spacing / 100; }
            set
            {
                _spacing = value;

                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }





        [SerializeField]
        private float _width = 23;
        public float Width
        {
            get
            {
                if (GetComponent<RectTransform>())
                    return GetComponent<RectTransform>().sizeDelta.x;
                return _width;
            }
            set
            {
                _width = value;

                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [SerializeField]
        private float _height = 4;
        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;

                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [SerializeField] float _depth = 4;
        public float Depth
        {
            get { return _depth; }
            set
            {
                _depth = value;

                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }




        [SerializeField]
        private LineSpacingStyle _lineSpacingStyle = LineSpacingStyle.maximum;
        /// <summary>
        /// Is not used for texts
        /// </summary>
        public LineSpacingStyle MyLineSpacingStyle
        {
            get { return _lineSpacingStyle; }
            set
            {
                _lineSpacingStyle = value;

                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }


        int currentLine = 0;
        int currentLetter;
        int currentWord;
#if MODULAR_3D_TEXT
        Modular3DText text;
#endif

        public List<Line> lines = new List<Line>();
        #endregion Variable Declaration






        public override void UpdateLayout(int startRepositioningFrom = 0)
        {
            if (transform.childCount == 0)
                return;

#if MODULAR_3D_TEXT
            text = GetComponent<Modular3DText>();
#endif

            bounds = GetAllChildBounds();
            lines = GetLines();

            GetInitialYSpaceInformation(out float oneLineHeight, out int totalAmountOfLineInOneZposition, out float oneLineDepth);

            float y = StartingY(oneLineHeight, totalAmountOfLineInOneZposition); //Starting Y is taken everytime depth changes
            float z = StartingZ(oneLineDepth, totalAmountOfLineInOneZposition);

            int currentLineNumberInCurrentZposition = 0;

            for (int currentLineNumber = 0; currentLineNumber < lines.Count; currentLineNumber++)//for each line
            {
                ProcessThisLine(ref oneLineHeight, currentLineNumber, out float justiceXMultiplier, out float x);

                if (ThisLineDoesntFitsInCurrentZposition(totalAmountOfLineInOneZposition, currentLineNumberInCurrentZposition))
                {
                    currentLineNumberInCurrentZposition = 0;
                    GetYandZpositionForNewDepth(oneLineHeight, totalAmountOfLineInOneZposition, currentLineNumber, ref y, ref z, oneLineDepth);
                }




                y -= (Spacing.y + oneLineHeight) / 2;
                for (int i = 0; i < lines[currentLineNumber].gameObjects.Count; i++) //for each char on the line
                {
                    Bounds bound = GetBound(lines[currentLineNumber].gameObjects[i].transform);

                    float spaceX = ((Spacing.x + bound.size.x) / 2) * justiceXMultiplier;
                    x += spaceX;

                    Vector3 targetPos = new Vector3(x - bound.center.x, y, z);

                    if (i >= startRepositioningFrom)
                        SetLocalPosition(currentLineNumber, i, targetPos);

                    x += spaceX;
                }
                y -= (Spacing.y + oneLineHeight) / 2;


                currentLineNumberInCurrentZposition++;
            }
        }

        private void GetYandZpositionForNewDepth(float oneLineHeight, int totalAmountOfLineInOneZposition, int lineCount, ref float y, ref float z, float oneLineDepth)
        {
            if (lines.Count - lineCount > totalAmountOfLineInOneZposition) //more than what can fit in one depth is left
            {
                y = StartingY(oneLineHeight, totalAmountOfLineInOneZposition);
                z += Spacing.z + oneLineDepth;
            }
            else
            {
                y = StartingY(oneLineHeight, lines.Count - lineCount);
                z += Spacing.z + oneLineDepth;
            }
        }



        /// <summary>
        /// Used by Text only
        /// </summary>
        /// <param name="meshLayouts"></param>
        /// <returns></returns>
        public override List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts)
        {
            if (meshLayouts.Count == 0)
                return null;

#if MODULAR_3D_TEXT
            text = GetComponent<Modular3DText>();
#endif

            bounds = GetAllChildBounds(meshLayouts);
            lines = GetLines(bounds, meshLayouts);

            GetInitialYSpaceInformation(out float oneLineHeight, out int totalAmountOfLineInOneZposition, out float oneLineDepth);

            float y = StartingY(oneLineHeight, totalAmountOfLineInOneZposition); //Starting Y is taken everytime depth changes
            float z = StartingZ(oneLineDepth, totalAmountOfLineInOneZposition);

            int currentLineNumberInCurrentZposition = 0;
            for (int currentLineNumber = 0; currentLineNumber < lines.Count; currentLineNumber++) //for each line
            {
                ProcessThisLine(ref oneLineHeight, currentLineNumber, out float justiceXMultiplier, out float x);

                if (ThisLineDoesntFitsInCurrentZposition(totalAmountOfLineInOneZposition, currentLineNumberInCurrentZposition))
                {
                    currentLineNumberInCurrentZposition = 0;
                    GetYandZpositionForNewDepth(oneLineHeight, totalAmountOfLineInOneZposition, currentLineNumber, ref y, ref z, oneLineDepth);
                }



                y -= (Spacing.y + oneLineHeight) / 2;
                for (int i = 0; i < lines[currentLineNumber].meshLayouts.Count; i++) //for each char on the line
                {
                    Bounds bound = GetBound(lines[currentLineNumber].meshLayouts[i]);

                    float spaceX = ((Spacing.x + bound.size.x) / 2) * justiceXMultiplier;
                    x += spaceX;


                    Vector3 targetPos = new Vector3(x - bound.center.x, y, z);

                    //if (targetPos != lines[lineCount].gameObjects[i].transform.localPosition)
                    lines[currentLineNumber].meshLayouts[i].position = targetPos;

                    x += spaceX;
                }
                y -= (Spacing.y + oneLineHeight) / 2;



                currentLineNumberInCurrentZposition++;
            }

            return meshLayouts;
        }




        #region Methods only called once for each layout update
        void ProcessThisLine(ref float thisLineHeight, int lineCount, out float justiceXMultiplier, out float x)
        {
            float spaceRequired = GetXSpaceRequired(lines[lineCount]);
            justiceXMultiplier = GetJusticeXMultiplier(spaceRequired);

            if (lines[lineCount].meshLayouts.Count > 0)
                x = StartingX(lines[lineCount].meshLayouts, spaceRequired);
            else
                x = StartingX(lines[lineCount].gameObjects, spaceRequired);

#if MODULAR_3D_TEXT
            if (!text && MyLineSpacingStyle == LineSpacingStyle.individual) //Mesh layout is only called by text, so this isn't required
#else
            if (MyLineSpacingStyle == LineSpacingStyle.individual) //Mesh layout is only called by text, so this isn't required
#endif
            {
                thisLineHeight = GetSpecificLineMaxYSpace(lines[lineCount].gameObjects);
#if UNITY_EDITOR
                lines[lineCount].individualYSpace = thisLineHeight;
#endif
            }
        }
        void GetInitialYSpaceInformation(out float oneLineHeight, out int totalAmountOfLineInOneZaxis, out float oneLineDepth)
        {
            oneLineHeight = GetLineHeight();


            //Get how many lines will fit.
            float totalYspaceTakenByAllLines = lines.Count * (Spacing.y + oneLineHeight);

            if (totalYspaceTakenByAllLines > Height)
                totalAmountOfLineInOneZaxis = Mathf.FloorToInt(Height / (Spacing.y + oneLineHeight));
            else
                totalAmountOfLineInOneZaxis = lines.Count;


            oneLineDepth = GetZspacing();
        }
        #endregion





        void SetLocalPosition(int lineCount, int i, Vector3 targetPos)
        {
            if (elementUpdater.module)
            {
                elementUpdater.module.UpdateLocalPosition(lines[lineCount].gameObjects[i].transform, elementUpdater.variableHolders, targetPos);
            }
            else
            {
                if (targetPos != lines[lineCount].gameObjects[i].transform.localPosition) //This is to avoid unnecessary marking things as changed in scene hierarchy
                    lines[lineCount].gameObjects[i].transform.localPosition = targetPos;
            }
        }











        float GetLineHeight()
        {
#if MODULAR_3D_TEXT
            if (MyLineSpacingStyle == LineSpacingStyle.maximum || text)
#else
            if (MyLineSpacingStyle == LineSpacingStyle.maximum)
#endif
                return GetMaxYSpace(bounds);
            else if (MyLineSpacingStyle == LineSpacingStyle.average)
                return GetAverageYSpace(bounds);

            //individual
            return 1;
        }
        float GetMaxYSpace(Bounds[] bounds)
        {
#if MODULAR_3D_TEXT
            if (text)
                if (text.Font)
                    return text.Font.lineHeight * text.FontSize.y / (Mathf.Clamp(text.Font.unitPerEM, 0.0001f, 1000000) * 8);
#endif

            float maxY = 0;

            for (int i = 0; i < bounds.Length; i++)
            {
                if (bounds[i].size.y > maxY)
                {
                    if (!IgnoreChildBoundAndLineBreak(bounds, i))
                    {
                        maxY = bounds[i].size.y;
                    }
                }
            }

            return maxY;
        }

        float GetJusticeXMultiplier(float spaceRequired)
        {
            if (ApplyHorizontalJustice(spaceRequired))
                return Width / spaceRequired;

            return 1;
        }

        /// <summary>
        /// Used by 3d Text only when single mesh is used
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        List<Line> GetLines(Bounds[] bounds, List<MeshLayout> meshLayouts)
        {
            List<Line> lines = new List<Line>();

            Line line = new Line();
            lines.Add(line);

            float x = 0;

            currentLetter = 0;
            currentWord = 0;
            currentLine = 0;

            bool checkedIfFits = false; //This checks if a word in text is already tested to fit.

            for (int i = 0; i < bounds.Length; i++)
            {
#if MODULAR_3D_TEXT
                IsThisAnewWord(text, ref checkedIfFits);
#else
                IsThisAnewWord(ref checkedIfFits);
#endif
                ///New line
                if (ItsALineBreak(meshLayouts, lines, ref currentLine, ref x, i))
                {
                    currentLetter = 0;
                    currentWord++;
                    continue;
                }

#if UNITY_EDITOR
                lines[currentLine].widthIfNextObjectWasHere = x + Spacing.x + bounds[i].size.x;
#endif

                //Debug.Log("current word number:" + currentWord);

                //Get current line number
                //Willl it be new line if current object is added
                if ((float)Math.Round(x + Spacing.x + bounds[i].size.x, 5) > Width) //New line 
                {
                    x = bounds[i].size.x + Spacing.x;
                    Line newLine = new Line();
                    lines.Add(newLine);

#if MODULAR_3D_TEXT
                    MoveWordToNextLine(meshLayouts, bounds, lines, currentLine, ref x, text, currentWord, ref checkedIfFits, i);
#else
                    MoveWordToNextLine(meshLayouts, bounds, lines, currentLine, ref x, currentWord, ref checkedIfFits, i);
#endif
                    //Debug.Log(" New line ");
                    currentLine++;
                }
                else
                {
                    x += bounds[i].size.x + Spacing.x;
#if UNITY_EDITOR
                    if (x != 0)
                        lines[currentLine].width = x;
#endif
                }

                lines[currentLine].meshLayouts.Add(meshLayouts[i]);
                currentLetter++;
            }

#if MODULAR_3D_TEXT
            if (text)
                lines = RemoveUnnecessarySpacesFromMeshLayouts(lines);
#endif
            if (lines.Count > 0)
            {
                //remove empty first line
                if (lines[0].gameObjects.Count == 0 && lines[0].meshLayouts.Count == 0)
                    lines.RemoveAt(0);

                if (lines.Count > 0) //this is because if the only text is space and the line is removed, there are no lines and returns null error
                {
                    //remove empty last line
                    if (lines[lines.Count - 1].gameObjects.Count == 0 && lines[lines.Count - 1].meshLayouts.Count == 0)
                        lines.RemoveAt(lines.Count - 1);
                }
            }


            return lines;
        }

        /// <summary>
        /// Used in all cases except when text has single mesh turned on
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        List<Line> GetLines()
        {
            List<Line> lines = new List<Line>();

            Line line = new Line();
            lines.Add(line);

            float x = 0;

            currentLetter = 0;
            currentWord = 0;
            currentLine = 0;

            bool checkedIfFits = false;

            for (int i = 0; i < transform.childCount; i++)
            {
#if MODULAR_3D_TEXT
                if (text)
                    if (!text.characterObjectList.Contains(transform.GetChild(i).gameObject))
                        continue;
                IsThisAnewWord(text, ref checkedIfFits);
#else
                IsThisAnewWord(ref checkedIfFits);
#endif

                ///New line
                if (ItsALineBreak(lines, ref currentLine, ref x, i))
                {
                    currentLetter = 0;
                    currentWord++;
                    continue;
                }

#if MODULAR_3D_TEXT
                if (!text && IgnoreChildBound(bounds, i))
#else
                if (IgnoreChildBound(bounds, i))
#endif
                    continue;


#if UNITY_EDITOR
                lines[currentLine].widthIfNextObjectWasHere = x + Spacing.x + bounds[i].size.x;
#endif
                //Get current line number
                //Willl it be new line if current object is added
                //Debug.Log((float)Math.Round(x + Spacing.x + bounds[i].size.x, 5));
                if ((float)Math.Round(x + Spacing.x + bounds[i].size.x, 5) > Width) //New line 
                {
                    x = bounds[i].size.x + Spacing.x;
                    Line nline = new Line();
                    lines.Add(nline);
                    //Debug.Log(text.wordArray[word] + " word for: " + currentLetter + " letter for: " + transform.GetChild(i), transform.GetChild(i));
#if MODULAR_3D_TEXT
                    MoveWordToNextLine(bounds, lines, currentLine, ref x, text, currentLetter, currentWord, ref checkedIfFits, i);
#else
                    MoveWordToNextLine(bounds, lines, currentLine, ref x, currentLetter, currentWord, ref checkedIfFits, i);
#endif

                    currentLine++;
                }
                else // in the same line
                {
                    x += bounds[i].size.x + Spacing.x;
#if UNITY_EDITOR
                    lines[currentLine].width = x;
#endif
                }

                lines[currentLine].gameObjects.Add(transform.GetChild(i).gameObject);
                currentLetter++;
            }


#if MODULAR_3D_TEXT
            if (text)
                lines = RemoveUnnecessarySpaces(lines);
#endif
            if (lines.Count > 0)
            {
                //remove empty first line
                if (lines[0].gameObjects.Count == 0 && lines[0].meshLayouts.Count == 0)
                    lines.RemoveAt(0);

                if (lines.Count > 0)
                {
                    //remove empty last line
                    if (lines[lines.Count - 1].gameObjects.Count == 0)
                        lines.RemoveAt(lines.Count - 1);
                }
            }

            return lines;
        }




        #region Position

        #region X space stuff
        float StartingX(List<MeshLayout> meshlayouts, float spaceRequired)
        {
            if (Anchor == Alignment.UpperLeft || Anchor == Alignment.MiddleLeft || Anchor == Alignment.LowerLeft)
                return (-Width / 2) - Spacing.x / 2;

            else if (Anchor == Alignment.UpperRight || Anchor == Alignment.MiddleRight || Anchor == Alignment.LowerRight)
            {
                if (!ApplyHorizontalJustice(spaceRequired))
                    return (Width / 2) - GetXSpaceRequired(meshlayouts) + Spacing.x / 2;
                else
                    return (-Width / 2) - Spacing.x / 2;
            }
            else
            {
                if (!ApplyHorizontalJustice(spaceRequired))
                    return -GetXSpaceRequired(meshlayouts) / 2;
                else
                    return (-Width / 2) - Spacing.x / 2;
            }
        }

        float StartingX(List<GameObject> gameObjects, float spaceRequired)
        {
            if (Anchor == Alignment.UpperLeft || Anchor == Alignment.MiddleLeft || Anchor == Alignment.LowerLeft)
                return (-Width / 2) - Spacing.x / 2;

            else if (Anchor == Alignment.UpperRight || Anchor == Alignment.MiddleRight || Anchor == Alignment.LowerRight)
            {
                if (!ApplyHorizontalJustice(spaceRequired))
                    return (Width / 2) - GetXSpaceRequired(gameObjects) + Spacing.x / 2;
                else
                    return (-Width / 2) - Spacing.x / 2;
            }

            else
            {
                if (!ApplyHorizontalJustice(spaceRequired))
                    return -GetXSpaceRequired(gameObjects) / 2;
                else
                    return (-Width / 2) - Spacing.x / 2;
            }
        }

        float GetXSpaceRequired(Line line)
        {
            if (line.gameObjects.Count > 0)
                return GetXSpaceRequired(line.gameObjects);
            else
                return GetXSpaceRequired(line.meshLayouts);
        }

        float GetXSpaceRequired(List<GameObject> targets)
        {
            float width = 0;

            for (int i = 0; i < targets.Count; i++)
            {
                width += GetBound(targets[i].transform).size.x + Spacing.x;
            }

            return width;
        }

        float GetXSpaceRequired(List<MeshLayout> targets)
        {
            float width = 0;

            for (int i = 0; i < targets.Count; i++)
            {
                width += GetBound(targets[i]).size.x + Spacing.x;
            }

            return width;
        }
        #endregion X space stuff

        #region Y space stuff
        /// <summary>
        /// 
        /// </summary>
        /// <param name="linesCount"></param>
        /// <param name="spacingY">Spacing Y selected in the inspector</param>
        /// <param name="ySpace"></param>
        /// <returns></returns>
        float StartingY(float ySpace, float maxLines)
        {
            if (IsUpperAlignment())
                return -StartingYforUpperAlignment();

            else if (IsMiddleAlignment())
                return -StartingYforMiddleAlignment(ySpace, maxLines);

            else // (Anchor == Alignment.LowerLeft || Anchor == Alignment.LowerCenter || Anchor == Alignment.LowerRight)
                return -StartingYforLowerAlignment(ySpace, maxLines);

        }
        float StartingYforUpperAlignment()
        {
            return (-Height / 2) - (Spacing.y / 2);
        }
        float StartingYforMiddleAlignment(float ySpace, float maxLines)
        {
#if MODULAR_3D_TEXT
            if (text || MyLineSpacingStyle != LineSpacingStyle.individual)
#else
            if (MyLineSpacingStyle != LineSpacingStyle.individual)
#endif
            {
                //float ySize = maxLines * (Spacing.y + ySpace);

                return -(maxLines * (Spacing.y + ySpace)) / 2;
            }
            return -GetTotalYSpaceTakenByCheckingEachLineIndividually(maxLines) / 2;
        }

        float StartingYforLowerAlignment(float ySpace, float maxLines)
        {
#if MODULAR_3D_TEXT
            if (text || MyLineSpacingStyle != LineSpacingStyle.individual)
#else
            if (MyLineSpacingStyle != LineSpacingStyle.individual)
#endif
            {
                float ySize = maxLines * (Spacing.y + ySpace);

                return (Height / 2) - ySize + Spacing.y / 2;
            }

            return (Height / 2) - GetTotalYSpaceTakenByCheckingEachLineIndividually(maxLines) + Spacing.y / 2;
        }

        float GetTotalYSpaceTakenByCheckingEachLineIndividually(float maxLines)
        {
            float ySpaceTaken = 0;
            //for (int i = 0; i < lines.Count; i++)
            for (int i = 0; i < maxLines; i++)
            {
                ySpaceTaken += GetSpecificLineMaxYSpace(lines[i].gameObjects) + Spacing.y;
            }
            return ySpaceTaken;
        }

        /// <summary>
        /// Isn't used by text
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        float GetAverageYSpace(Bounds[] bounds)
        {
            float totalY = 0;
            float totalElements = 0;

            for (int i = 0; i < bounds.Length; i++)
            {
                if (!IgnoreChildBoundAndLineBreak(bounds, i))
                {
                    totalY += bounds[i].size.y;
                    totalElements++;
                }
            }

            return totalY / totalElements;
        }
        /// <summary>
        /// Isn't used by text
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        float GetSpecificLineMaxYSpace(List<GameObject> itemsInLine)
        {
            float maxY = 0;

            for (int i = 0; i < itemsInLine.Count; i++)
            {
                Bounds bound = GetBound(itemsInLine[i].transform);
                if (bound.size.y > maxY)
                {
                    if (!IgnoreChildBoundAndLineBreak(bounds, i))
                    {
                        maxY = bound.size.y;
                    }
                }
            }

            return maxY;
        }
        #endregion Y space stuff

        float GetZspacing()
        {
            return GetAverageZSpace();
        }
        float GetAverageZSpace()
        {
            float totalZ = 0;
            float totalElements = 0;

            for (int i = 0; i < bounds.Length; i++)
            {
                if (!IgnoreChildBoundAndLineBreak(bounds, i))
                {
                    totalZ += bounds[i].size.z;
                    totalElements++;
                }
            }

            return totalZ / totalElements;
        }
        float StartingZ(float oneLineDepth, int totalAmountOfLineInOneZposition)
        {
            if (DepthAlignment == DepthAlignment.front)
                return (-Depth / 2) + (oneLineDepth / 2);
            else if (DepthAlignment == DepthAlignment.middle)
                return (-TotalZspaceTaken(oneLineDepth, totalAmountOfLineInOneZposition) / 2) + (Spacing.z + oneLineDepth) / 2;
            else
            {
                float zSize = TotalZspaceTaken(oneLineDepth, totalAmountOfLineInOneZposition);
                return (Depth / 2) - zSize + (Spacing.z + oneLineDepth / 2);
            }
        }

        private float TotalZspaceTaken(float oneLineDepth, int totalAmountOfLineInOneZposition)
        {
            return (Mathf.CeilToInt((float)lines.Count / totalAmountOfLineInOneZposition) * (Spacing.z + oneLineDepth));
        }


        bool IsUpperAlignment() => Anchor == Alignment.UpperLeft || Anchor == Alignment.UpperCenter || Anchor == Alignment.UpperRight;
        bool IsMiddleAlignment() => Anchor == Alignment.MiddleLeft || Anchor == Alignment.MiddleCenter || Anchor == Alignment.MiddleRight;

        #endregion Pos


        bool ApplyHorizontalJustice(float spaceRequired)
        {
            if (!JusticeHorizontal)
            {
                return false;
            }
            else
            {
                if ((spaceRequired / Width) * 100 >= JusticeHorizontalPercent)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// For text when combine mesh is on
        /// </summary>
        /// <param name="meshLayouts"></param>
        /// <param name="lines"></param>
        /// <param name="currentLine"></param>
        /// <param name="x"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        bool ItsALineBreak(List<MeshLayout> meshLayouts, List<Line> lines, ref int currentLine, ref float x, int i)
        {
            if (meshLayouts[i].lineBreak)
            {
                lines[currentLine].meshLayouts.Add(meshLayouts[i]);

                x = 0;
                Line nline = new Line();
                lines.Add(nline);
                currentLine++;

                return true;
            }
            return false;
        }

        /// <summary>
        /// For text when combine mesh is on
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="currentLine"></param>
        /// <param name="x"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        bool ItsALineBreak(List<Line> lines, ref int currentLine, ref float x, int i)
        {
            if (transform.GetChild(i).GetComponent<LayoutElement>())
            {
                if (transform.GetChild(i).GetComponent<LayoutElement>().lineBreak)
                {
                    lines[currentLine].gameObjects.Add(transform.GetChild(i).gameObject);

                    x = 0;
                    Line nline = new Line();
                    lines.Add(nline);
                    currentLine++;

                    return true;
                }
            }
            return false;
        }





#if MODULAR_3D_TEXT
        void IsThisAnewWord(Modular3DText text, ref bool checkedIfFits)
#else
        void IsThisAnewWord(ref bool checkedIfFits)
#endif
        {
#if MODULAR_3D_TEXT
            if (text)
            {
                if (text.wordArray != null)
                {
                    if (text.wordArray.Length > currentWord)
                    {
                        if (text.wordArray[currentWord] == string.Empty)
                        {
                            currentLetter = 0;
                            currentWord++;
                            checkedIfFits = false;
                        }
                    }
                    if (text.wordArray.Length > currentWord)
                    {
                        if (currentLetter >= text.wordArray[currentWord].Length)
                        {
                            currentLetter = 0;
                            currentWord++;
                            checkedIfFits = false;
                        }
                    }
                }
            }
#endif
        }
#if MODULAR_3D_TEXT
        void MoveWordToNextLine(List<MeshLayout> meshLayouts, Bounds[] bounds, List<Line> lines, int currentLine, ref float x, Modular3DText text, int word, ref bool checkedIfFits, int i)
#else
        void MoveWordToNextLine(List<MeshLayout> meshLayouts, Bounds[] bounds, List<Line> lines, int currentLine, ref float x, int word, ref bool checkedIfFits, int i)
#endif
        {
#if MODULAR_3D_TEXT
            if (text)
            {
                if (currentLetter > 0)
                {
                    //Debug.Log("Current letter number: " + letter);
                    if (checkedIfFits == false)
                    {
#if UNITY_EDITOR
                        lines[currentLine].editorNote = "Checking if fits";
#endif
                        if (WordIsntTooBigForOneLine(bounds, text, i, word, currentLetter))
                        {
#if UNITY_EDITOR
                            lines[currentLine].editorNote = "Word isn't too big";
#endif
                            List<MeshLayout> temp = new List<MeshLayout>();

                            //if the a letter of a word is pushed to a new line,
                            //scroll through  all previous letters in current word
                            //and add them to the new line
                            for (int j = currentLetter; j >= 0; j--)
                            {
                                if (lines[currentLine].meshLayouts.Count > 0)
                                {
                                    MeshLayout g = lines[currentLine].meshLayouts[lines[currentLine].meshLayouts.Count - 1];
                                    lines[currentLine].meshLayouts.Remove(g);
                                    x += bounds[i - j].size.x + Spacing.x; //add the size of the letters for new word's calculation
                                    temp.Add(g);
                                }
                            }
                            for (int k = temp.Count - 1; k >= 0; k--)
                            {
                                lines[currentLine + 1].meshLayouts.Add(temp[k]);
                            }
                        }
                        else
                        {
                            //Debug.Log("Word too big to fit");
                        }
                    }
                }
            }
#endif
        }
#if MODULAR_3D_TEXT
        void MoveWordToNextLine(Bounds[] bounds, List<Line> lines, int currentLine, ref float x, Modular3DText text, int letter, int word, ref bool checkedIfFits, int i)
#else
        void MoveWordToNextLine(Bounds[] bounds, List<Line> lines, int currentLine, ref float x, int letter, int word, ref bool checkedIfFits, int i)
#endif
        {
#if MODULAR_3D_TEXT
            if (text)
            {
                //Debug.Log(letter + " letter " + transform.GetChild(i) + " is on new line");
                if (letter > 0)
                {
                    if (checkedIfFits == false)
                    {
                        if (WordIsntTooBigForOneLine(bounds, text, i, word, letter))
                        {
                            List<GameObject> temp = new List<GameObject>();

                            //if the a letter of a word is pushed to a new line,
                            //scroll through  all previous letters in current word
                            //and add them to the new line
                            for (int j = letter; j >= 0; j--)
                            {
                                if (lines[currentLine].gameObjects.Count > 0)
                                {
                                    GameObject g = lines[currentLine].gameObjects[lines[currentLine].gameObjects.Count - 1];
                                    lines[currentLine].gameObjects.Remove(g);
                                    x += bounds[i - j].size.x + Spacing.x;
                                    temp.Add(g);
                                }
                            }

                            for (int k = temp.Count - 1; k >= 0; k--)
                            {
                                lines[currentLine + 1].gameObjects.Add(temp[k]);
                            }
                        }

                        checkedIfFits = true;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Bool because the updatelayout is called by update text
        /// If returns true, the layout has already been updated
        /// </summary>
        /// <returns></returns>
        bool UpdateTextIfRequired()
        {
#if MODULAR_3D_TEXT
            if (GetComponent<Modular3DText>())
            {
                if (!GetComponent<Modular3DText>().ShouldItCreateChild())
                {
                    GetComponent<Modular3DText>().CleanUpdateText();
                    return true;
                }
            }
#endif
            return false;
        }

#if MODULAR_3D_TEXT
        /// <summary>
        /// If the word is too big to fit in a single line
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="text"></param>
        /// <param name="currentChildNumber"></param>
        /// <param name="word"></param>
        /// <param name="letter"></param>
        /// <returns></returns>
        bool WordIsntTooBigForOneLine(Bounds[] bounds, Modular3DText text, int currentChildNumber, int word, int letter)
#else
        bool WordIsntTooBigForOneLine(Bounds[] bounds, int currentChildNumber, int word, int letter)
#endif
        {
#if MODULAR_3D_TEXT
            if (text.wordArray == null)
                return false;


            if (word >= text.wordArray.Length)
            {
                //Debug.Log("word index:" + word + " | Text's word array length: " + text.wordArray.Length);
                return true;
            }

            int wordStartsAtChildIndex = currentChildNumber - letter;
            int wordEndssAtChildIndex = currentChildNumber - letter + text.wordArray[word].Length - 1;

            //Debug.Log("word starts at index: " + wordStartsAtChildIndex + " word ends at index:" + wordEndssAtChildIndex);

            float x = 0;
            for (int i = wordStartsAtChildIndex; i <= wordEndssAtChildIndex; i++)
            {
                if (bounds.Length <= i)
                {
                    return false;
                }

                if (bounds.Length > i)
                    x += bounds[i].size.x;

                if (x > Width)
                {
                    return false;
                }
            }
#endif
            return true;
        }
#if MODULAR_3D_TEXT
        private List<Line> RemoveUnnecessarySpacesFromMeshLayouts(List<Line> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Line currentLine = lines[i];
                if (currentLine.meshLayouts.Count > 0)
                {
                    if (i != 0 && currentLine.meshLayouts[0].mesh == null)
                    {
                        currentLine.meshLayouts.RemoveAt(0);
                    }


                    if (currentLine.meshLayouts.Count > 1)
                    {
                        int lastIndex = currentLine.meshLayouts.Count - 1;
                        MeshLayout meshLayout = currentLine.meshLayouts[lastIndex];

                        if (meshLayout.mesh == null)
                        {
                            currentLine.meshLayouts.RemoveAt(lastIndex);
                        }
                        else if (meshLayout.lineBreak)
                        {
                            currentLine.meshLayouts.RemoveAt(lastIndex);
                            lastIndex--;
                            if (lastIndex > 0)
                            {
                                if (currentLine.meshLayouts[lastIndex].mesh == null)
                                {
                                    currentLine.gameObjects.RemoveAt(lastIndex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }

            return lines;
        }

        private List<Line> RemoveUnnecessarySpaces(List<Line> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Line currentLine = lines[i];
                if (currentLine.gameObjects.Count > 0) //should never be 0. Just incase
                {
                    LayoutElement firstElement = currentLine.gameObjects[0].GetComponent<LayoutElement>();
                    if (firstElement)
                    {
                        if (firstElement.space)
                        {
                            currentLine.gameObjects.RemoveAt(0);
                        }
                    }


                    if (currentLine.gameObjects.Count > 1)
                    {
                        int lastIndex = currentLine.gameObjects.Count - 1;
                        LayoutElement lastElement = currentLine.gameObjects[lastIndex].GetComponent<LayoutElement>();

                        if (lastElement)
                        {
                            //if (lines[i].gameObjects[lastIndex].name == "Space")
                            if (lastElement.space)
                            {
                                currentLine.gameObjects.RemoveAt(lastIndex);
                            }
                            else if (lastElement.lineBreak)
                            {
                                currentLine.gameObjects.RemoveAt(lastIndex);
                                lastIndex--;
                                if (lastIndex > 0)
                                {
                                    LayoutElement newLastElement = currentLine.gameObjects[lastIndex].GetComponent<LayoutElement>();
                                    if (newLastElement)
                                    {
                                        if (newLastElement.space)
                                        {
                                            currentLine.gameObjects.RemoveAt(lastIndex);
                                        }
                                    }
                                }
                            }
                        }


                    }
                }
                else
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }

            return lines;
        }

#endif



        bool ThisLineDoesntFitsInCurrentZposition(int totalAmountOfLineInOneZposition, int currentLineNumberInCurrentZposition) => currentLineNumberInCurrentZposition + 1 > totalAmountOfLineInOneZposition;




#if UNITY_EDITOR
        /// <summary>
        /// Draws the width and height
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1, 1, 1, 0.75f);

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(Width, Height, Depth));
        }
#endif
    }

    public enum DepthAlignment
    {
        front,
        middle,
        back
    }

}
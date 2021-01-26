using UnityEngine;
using UnityEditor;
using EditorGUITable;


[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]
public class CustomTerrainEditor : Editor
{
    // properties
    SerializedProperty randomHeightRange;
    SerializedProperty heightMapScale;
    SerializedProperty heightMapImage;
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOffsetY;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;
    SerializedProperty resetTerrain;
    SerializedProperty voronoiFallOff;
    SerializedProperty voronoiDropOff;
    SerializedProperty voronoiMinHeight;
    SerializedProperty voronoiMaxHeight;
    SerializedProperty voronoiPeaks;
    SerializedProperty voronoiType;
    SerializedProperty MPDheightMin;
    SerializedProperty MPDheightMax;
    SerializedProperty MPDheightDampenerPower;
    SerializedProperty MPDroughness;
    SerializedProperty waterHeight;
    SerializedProperty waterGO;
    SerializedProperty smoothAmount;
    SerializedProperty shorelineMaterial;
    SerializedProperty foamSize;
    SerializedProperty erosionType;
    SerializedProperty erosionStrength;
    SerializedProperty erosionAmount;
    SerializedProperty springsPerRiver;
    SerializedProperty solubility;
    SerializedProperty droplets;
    SerializedProperty erosionSmoothAmount;
    SerializedProperty WindDir;


    GUITableState vegetationTable;
    SerializedProperty vegetations;
    SerializedProperty maximumTree;
    SerializedProperty treeSpacing;

    GUITableState detailTable;
    SerializedProperty details;
    SerializedProperty maximumDetails;
    SerializedProperty detailsSpacing;

    GUITableState splatMapTable;
    SerializedProperty splatHeight;

    GUITableState perlinParameterTable;
    SerializedProperty PerlinParameters;

    //fold out
    bool showRandom = false;
    bool showLoadHeights = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlin = false;
    bool showVoronoi = false;
    bool showMidPointDisplacement=false;
    bool showSmooth = false;
    bool showSplatMaps = false;
    bool showheights = false;
    bool showVegetation=false;
    bool showDetails = false;
    bool showWater = false;
    bool showErosion = false;


    //HeighMap Texture
    Texture2D hmTexture;
    string fileName = "myTerrainTexture";

    private void OnEnable()
    {
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        heightMapScale = serializedObject.FindProperty("heightMapScale");
        heightMapImage = serializedObject.FindProperty("heightMapImage");
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        resetTerrain = serializedObject.FindProperty("resetTerrain");
        perlinParameterTable = new GUITableState("perlinParameterTable");
        PerlinParameters = serializedObject.FindProperty("perlinParameters");
        voronoiDropOff = serializedObject.FindProperty("voronoiDropOff");
        voronoiFallOff = serializedObject.FindProperty("voronoiFallOff");
        voronoiMinHeight = serializedObject.FindProperty("voronoiMinHeight");
        voronoiMaxHeight = serializedObject.FindProperty("voronoiMaxHeight");
        voronoiPeaks = serializedObject.FindProperty("voronoiPeaks");
        voronoiType = serializedObject.FindProperty("voronoiType");
        MPDheightMin = serializedObject.FindProperty("MPDheightMin");
        MPDheightMax = serializedObject.FindProperty("MPDheightMax");
        MPDheightDampenerPower = serializedObject.FindProperty("MPDheightDampenerPower");
        MPDroughness = serializedObject.FindProperty("MPDroughness");
        smoothAmount = serializedObject.FindProperty("smoothAmount");
        splatMapTable = new GUITableState("splatMapTable");
        splatHeight = serializedObject.FindProperty("splatHeights");
        vegetationTable = new GUITableState("vegetationTable");
        vegetations = serializedObject.FindProperty("vegetations");
        maximumTree= serializedObject.FindProperty("maximumTree");
        treeSpacing= serializedObject.FindProperty("treeSpacing");
        detailTable = new GUITableState("detailTable");
        details = serializedObject.FindProperty("details");
        maximumDetails = serializedObject.FindProperty("maximumDetails");
        detailsSpacing = serializedObject.FindProperty("detailsSpacing");
        waterHeight = serializedObject.FindProperty("waterHeight");
        waterGO = serializedObject.FindProperty("waterGO");
        shorelineMaterial= serializedObject.FindProperty("shorelineMaterial");
        foamSize = serializedObject.FindProperty("foamSize");
        erosionType= serializedObject.FindProperty("erosionType");
        erosionStrength=serializedObject.FindProperty("erosionStrength");
        springsPerRiver= serializedObject.FindProperty("springsPerRiver");
        solubility= serializedObject.FindProperty("solubility");
        droplets= serializedObject.FindProperty("droplets");
        erosionSmoothAmount= serializedObject.FindProperty("erosionSmoothAmount");
        erosionAmount= serializedObject.FindProperty("erosionAmount");
        WindDir = serializedObject.FindProperty("WindDir");



        hmTexture = new Texture2D(513, 513, TextureFormat.ARGB32, false);
;    }
    Vector2 scrollPos;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CustomTerrain terrain = (CustomTerrain)target;

        //Scrollbar Starting Code
        Rect r = EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(r.width), GUILayout.Height(r.height));
        EditorGUI.indentLevel++;


        EditorGUILayout.PropertyField(resetTerrain);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Modify Terrain", EditorStyles.boldLabel);
        showRandom = EditorGUILayout.Foldout(showRandom, "Random");
        if(showRandom)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomHeightRange);
            if(GUILayout.Button("Random Heights"))
            {
                terrain.RandomTerrain();
            }
        }
        showLoadHeights = EditorGUILayout.Foldout(showLoadHeights, "Load Heights");
        if(showLoadHeights)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Load Heights from Texture", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(heightMapImage);
            EditorGUILayout.PropertyField(heightMapScale);
            if(GUILayout.Button("Load Texture"))
            {
                terrain.LoadTexture();
            }
        }
        showPerlinNoise = EditorGUILayout.Foldout(showPerlinNoise, "Single Perlin Noise");
        if (showPerlinNoise)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Perlin Noise", EditorStyles.boldLabel);
            EditorGUILayout.Slider(perlinXScale,0,1,new GUIContent("X Scale"));
            EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("Y Scale"));
            EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("Offset X"));
            EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("Offset Y"));
            EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
            EditorGUILayout.Slider(perlinPersistance, 1, 10, new GUIContent("Persistance"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));
            if (GUILayout.Button("Perlin"))
            {
                terrain.Perlin();
            }
        }
        showMultiplePerlin= EditorGUILayout.Foldout(showMultiplePerlin, "Multiple Perlin Noise");
        if (showMultiplePerlin)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Multiple Perlin Noise", EditorStyles.boldLabel);
            perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable, PerlinParameters);
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("+"))
            {
                terrain.AddNewPerlin();
            }
            if(GUILayout.Button("-"))
            {
                terrain.RemovePerlin();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Multiple Perlin"))
            {
                terrain.MultiplePerlinTerrain();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Create Montains", EditorStyles.boldLabel);
        showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");
        if(showVoronoi)
        {
            EditorGUILayout.IntSlider(voronoiPeaks, 1, 10, new GUIContent("Peak Count"));
            EditorGUILayout.Slider(voronoiFallOff, 0, 10, new GUIContent("Fall Off"));
            EditorGUILayout.Slider(voronoiDropOff, 0, 10, new GUIContent("DropOff"));
            EditorGUILayout.Slider(voronoiMinHeight, 0, 1, new GUIContent("Min Height"));
            EditorGUILayout.Slider(voronoiMaxHeight, 0, 1, new GUIContent("Max Height"));
            EditorGUILayout.PropertyField(voronoiType);
            if (GUILayout.Button("Voronoi"))
            {
                terrain.Voronoi();
            }
        }
        showMidPointDisplacement = EditorGUILayout.Foldout(showMidPointDisplacement, "MidPoint Displacement");
        if (showMidPointDisplacement)
        {
            EditorGUILayout.PropertyField(MPDheightMin);
            EditorGUILayout.PropertyField(MPDheightMax);
            EditorGUILayout.PropertyField(MPDheightDampenerPower);
            EditorGUILayout.PropertyField(MPDroughness);
            if (GUILayout.Button("MidPoint Displacement"))
            {
                terrain.MidPointMisplacement();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Apply Textures", EditorStyles.boldLabel);
        showSplatMaps = EditorGUILayout.Foldout(showSplatMaps, "Splat Maps");
        if(showSplatMaps)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Splat Maps", EditorStyles.boldLabel);
            splatMapTable = GUITableLayout.DrawTable(splatMapTable, splatHeight);
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewSplatHeight();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemoveSplatHeight();
            }
            EditorGUILayout.EndHorizontal();
            if(GUILayout.Button("Apply SplatMaps"))
            {
                terrain.SplatMaps();
                GUIUtility.ExitGUI();
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Vegetation and Details", EditorStyles.boldLabel);
        showVegetation = EditorGUILayout.Foldout(showVegetation, "Vegetation");
        if (showVegetation)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Vegetation", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maximumTree, 0, 10000, new GUIContent("Maximum trees"));
            EditorGUILayout.IntSlider(treeSpacing, 0, 20, new GUIContent("Tree spacing"));
            vegetationTable = GUITableLayout.DrawTable(vegetationTable, vegetations);
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewVegetation();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemoveVegetation();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Plant Vegetation"))
            {
                terrain.PlantVegetation();
                GUIUtility.ExitGUI();
            }
        }
        showDetails = EditorGUILayout.Foldout(showDetails, "Details");
        if (showDetails)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Details", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maximumDetails, 0, 10000, new GUIContent("Maximum Details"));
            EditorGUILayout.IntSlider(detailsSpacing, 0, 20, new GUIContent("Details spacing"));
            vegetationTable = GUITableLayout.DrawTable(detailTable, details);
            terrain.GetComponent<Terrain>().detailObjectDistance = maximumDetails.intValue;
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewDetail();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemoveDetail();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Add the Details"))
            {
                terrain.AddDetails();
                GUIUtility.ExitGUI();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Water and shores", EditorStyles.boldLabel);
        showWater = EditorGUILayout.Foldout(showWater, "Water");
        if (showWater)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Water", EditorStyles.boldLabel);
            EditorGUILayout.Slider(waterHeight, 0, 1, new GUIContent("Water Height"));
            EditorGUILayout.PropertyField(waterGO);

            if (GUILayout.Button("Add Water"))
            {
                terrain.AddWater();
            }
            GUILayout.Label("Shoreline foam", EditorStyles.boldLabel);
            EditorGUILayout.Slider(foamSize, 1, 50, new GUIContent("Foam size"));
            EditorGUILayout.PropertyField(shorelineMaterial);
            if (GUILayout.Button("Add shoreline"))
            {
                terrain.DrawShoreline();
            }

        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Enviromental erosions", EditorStyles.boldLabel);
        showErosion = EditorGUILayout.Foldout(showErosion, "Erosion");
        if (showErosion)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Erosions", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(erosionType);
            EditorGUILayout.Slider(erosionStrength, 0, 1, new GUIContent("Erosion Strenght"));
            EditorGUILayout.Slider(erosionAmount, 0, 1, new GUIContent("Erosion Amount"));
            EditorGUILayout.IntSlider(droplets, 1, 500, new GUIContent("Droplets"));
            EditorGUILayout.Slider(solubility, 0.001f, 1, new GUIContent("Solubility"));
            EditorGUILayout.IntSlider(springsPerRiver, 0, 20, new GUIContent("Spring Per River"));
            EditorGUILayout.Slider(WindDir, 0, 360, new GUIContent("Win Direction"));
            EditorGUILayout.IntSlider(erosionSmoothAmount, 0, 10, new GUIContent("Smooth Amout"));
            

            if (GUILayout.Button("Erode"))
            {
                terrain.Erode();
            }
        }


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Common Stuff", EditorStyles.boldLabel);

        showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth Terrain");
        if (showSmooth)
        {
            EditorGUILayout.IntSlider(smoothAmount, 1, 10, new GUIContent("smoothAmount"));
            if (GUILayout.Button("Smooth"))
            {
                terrain.Smooth();
            }
        }
        showheights = EditorGUILayout.Foldout(showheights, "Display and save height map");
        if(showheights)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            fileName = EditorGUILayout.TextField("Texture Name", fileName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int hmtSize = (int)(EditorGUIUtility.currentViewWidth - 100);
            GUILayout.Label(hmTexture, GUILayout.Width(hmtSize), GUILayout.Height(hmtSize));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh", GUILayout.Width(hmtSize)))
            {
                float[,] heightMap = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
                for(int y=0;y<terrain.terrainData.heightmapResolution; y++)
                {
                    for(int x=0;x<terrain.terrainData.heightmapResolution; x++)
                    {
                        hmTexture.SetPixel(x,y,new Color(heightMap[x,y],heightMap[x,y],heightMap[x,y],1));
                    }
                }
                hmTexture.Apply();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.Width(hmtSize)))
            {
                byte[] bytes = hmTexture.EncodeToPNG();
                System.IO.Directory.CreateDirectory(Application.dataPath + "/SavedTextures/");
                System.IO.File.WriteAllBytes(Application.dataPath + "/SavedTextures/" + fileName + ".png", bytes);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }
        if (GUILayout.Button("Reset Terrain"))
        {
            terrain.ResetTerrain();
        }

        //Scrollbar ending code
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

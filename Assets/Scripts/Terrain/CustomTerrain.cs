using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


[ExecuteInEditMode]
public class CustomTerrain : MonoBehaviour
{
    public Vector2 randomHeightRange = new Vector2(0, 0.1f);
    public Texture2D heightMapImage;
    public Vector3 heightMapScale = new Vector3(1, 1, 1);

    public bool resetTerrain = true;

    //PERLIN NOISE
    public float perlinXScale = 0.01f;
    public float perlinYScale = 0.01f;
    public int perlinOffsetX = 0;
    public int perlinOffsetY = 0;
    public int perlinOctaves = 3;
    public float perlinPersistance = 8;
    public float perlinHeightScale = 0.09f;

    //MULTIPLE PERLIN
    [System.Serializable]
    public class PerlinParameters
    {
        public float perlinXScale = 0.01f;
        public float perlinYScale = 0.01f;
        public int perlinOffsetX = 0;
        public int perlinOffsetY = 0;
        public int perlinOctaves = 3;
        public float perlinPersistance = 8;
        public float perlinHeightScale = 0.09f;
        public bool remove = false;
    }

    public List<PerlinParameters> perlinParameters = new List<PerlinParameters>()
    {
        new PerlinParameters()
    };

    //SPLATMAPS
    [System.Serializable]
    public class SplatHeights
    {
        public Texture2D texture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public Vector2 Slope = new Vector2(0.0f, 1.5f);
        public Vector2 tileOffset = new Vector2(0, 0);
        public Vector2 tileSize = new Vector2(50, 50);
        public float offset = 0.01f;
        public Vector2 NoiseScale=new Vector2(0.01f, 0.01f);
        public float splatNoiseScaler = 0.1f;
        public bool remove = false;

    }

    public List<SplatHeights> splatHeights = new List<SplatHeights>()
    {
        new SplatHeights()
    };

    //Vegetation
    [System.Serializable]
    public class Vegetation
    {
        public GameObject mesh;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0;
        public float maxSlope = 90;
        public float minScale = 0.5f;
        public float maxScale = 1.0f;
        public Color colour1 = Color.white;
        public Color colour2 = Color.white;
        public Color lightColour = Color.white;
        public float minRotation = 0;
        public float maxRotation = 360;
        public float density = 0.5f;
        public bool remove = false;

    }

    public List<Vegetation> vegetations = new List<Vegetation>()
    {
        new Vegetation()
    };

    public int maximumTree = 5000;
    public int treeSpacing = 5;

    //Details

    [System.Serializable]
    public class Details
    {
        public GameObject prototype=null;
        public Texture2D prototypeTexture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0;
        public float maxSlope = 90;
        public float overlap = 0.01f;
        public float feather = 0.05f;
        public float density = 0.5f;
        public bool remove = false;
    }

    public List<Details> details = new List<Details>()
    {
        new Details()
    };
    public int maximumDetails=5000;
    public int detailsSpacing=5;


    //VORONOI
    public float voronoiFallOff=0.2f;
    public float voronoiDropOff=0.6f;
    public float voronoiMinHeight=0.1f;
    public float voronoiMaxHeight=0.5f;
    public int voronoiPeaks=5;
    public enum VoronoiType { Linear=0,Power=1,Combined=2,Sin=3,SinPow=4,CosPow=5 }
    public VoronoiType voronoiType=VoronoiType.Linear;

    //Midpoint Displacement
    public float MPDheightMin=-2;
    public float MPDheightMax=2f;
    public float MPDheightDampenerPower=2.0f;
    public float MPDroughness=2.0f;

    //Smooth
    public int smoothAmount = 2;


    public Terrain terrain;
    public TerrainData terrainData;


    float[,] GetHeightMap()
    {
        if (!resetTerrain)
        {
            return terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        }
        else
        {
            return new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        }
    }

    List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height)
    {
        List<Vector2> neighbours = new List<Vector2>();
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (!(x == 0 && y == 0))
                {
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                                                Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbours.Contains(nPos))
                    {
                        neighbours.Add(nPos);
                    }
                        
                }
            }
        }
        return neighbours;
    }

    public void AddDetails()
    {
        DetailPrototype[] newDetailPrototypes;
        newDetailPrototypes = new DetailPrototype[details.Count];
        int dindex = 0;
        foreach(Details d in details)
        {
            newDetailPrototypes[dindex] = new DetailPrototype();
            newDetailPrototypes[dindex].prototype = d.prototype;
            newDetailPrototypes[dindex].prototypeTexture = d.prototypeTexture;
            newDetailPrototypes[dindex].healthyColor = Color.white;
            if(newDetailPrototypes[dindex].prototype)
            {
                newDetailPrototypes[dindex].usePrototypeMesh = true;
                newDetailPrototypes[dindex].renderMode = DetailRenderMode.VertexLit;
            }
            else
            {
                newDetailPrototypes[dindex].usePrototypeMesh = false;
                newDetailPrototypes[dindex].renderMode = DetailRenderMode.GrassBillboard;
            }
            dindex++;
        }
        terrainData.detailPrototypes = newDetailPrototypes;

        for(int i=0;i<terrainData.detailPrototypes.Length;i++)
        {
            int[,] detailsMap = new int[terrainData.detailWidth, terrainData.detailHeight];
            for(int y=0;y<terrainData.detailHeight;y+=detailsSpacing)
            {
                for(int x=0;x<terrainData.detailWidth;x+=detailsSpacing)
                {
                    if(UnityEngine.Random.Range(0.0f,1.0f)>details[i].density)
                    {
                        continue;
                    }
                    detailsMap[y, x] = 1;
                }
            }
            terrainData.SetDetailLayer(0, 0, i, detailsMap);
        }
    }

    public void AddNewDetail()
    {
        details.Add(new Details());
    }

    public void RemoveDetail()
    {
        List<Details> keptDetails = new List<Details>();
        for (int i = 0; i < details.Count; i++)
        {
            if (!details[i].remove)
            {
                keptDetails.Add(details[i]);
            }
        }
        if (keptDetails.Count == 0) //don't want to keep any
        {
            keptDetails.Add(details[0]); //add at least 1
        }
        details = keptDetails;
    }

    public void PlantVegetation()
    {
        TreePrototype[] newTreePrototypes;
        newTreePrototypes = new TreePrototype[vegetations.Count];
        int tindex = 0;
        foreach (Vegetation t in vegetations)
        {
            newTreePrototypes[tindex] = new TreePrototype();
            newTreePrototypes[tindex].prefab = t.mesh;
            tindex++;
        }
        terrainData.treePrototypes = newTreePrototypes;

        List<TreeInstance> allVegetation = new List<TreeInstance>();
        for (int z = 0; z < terrainData.size.z; z += treeSpacing)
        {
            for (int x = 0; x < terrainData.size.x; x += treeSpacing)
            {
                for (int tp = 0; tp < terrainData.treePrototypes.Length; tp++)
                {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) > vegetations[tp].density) break;

                    float thisHeight = terrainData.GetHeight(x, z) / terrainData.size.y;
                    float thisHeightStart = vegetations[tp].minHeight;
                    float thisHeightEnd = vegetations[tp].maxHeight;

                    float steepness = terrainData.GetSteepness(x / (float)terrainData.size.x,
                                                               z / (float)terrainData.size.z);

                    if ((thisHeight >= thisHeightStart && thisHeight <= thisHeightEnd) &&
                        (steepness >= vegetations[tp].minSlope && steepness <= vegetations[tp].maxSlope))
                    {
                        TreeInstance instance = new TreeInstance();
                        instance.position = new Vector3((x + UnityEngine.Random.Range(-5.0f, 5.0f)) / terrainData.size.x,
                                                        terrainData.GetHeight(x, z) / terrainData.size.y,
                                                        (z + UnityEngine.Random.Range(-5.0f, 5.0f)) / terrainData.size.z);

                        Vector3 treeWorldPos = new Vector3(instance.position.x * terrainData.size.x,
                            instance.position.y * terrainData.size.y,
                            instance.position.z * terrainData.size.z)
                                                         + this.transform.position;

                        RaycastHit hit;
                        int layerMask = 1 << terrainLayer;

                        if (Physics.Raycast(treeWorldPos + new Vector3(0, 10, 0), -Vector3.up, out hit, 100, layerMask) ||
                            Physics.Raycast(treeWorldPos - new Vector3(0, 10, 0), Vector3.up, out hit, 100, layerMask))
                        {
                            float treeHeight = (hit.point.y - this.transform.position.y) / terrainData.size.y;
                            instance.position = new Vector3(instance.position.x,
                                                             treeHeight,
                                                             instance.position.z);

                            instance.rotation = UnityEngine.Random.Range(vegetations[tp].minRotation,
                                                                         vegetations[tp].maxRotation);
                            instance.prototypeIndex = tp;
                            instance.color = Color.Lerp(vegetations[tp].colour1,
                                                        vegetations[tp].colour2,
                                                        UnityEngine.Random.Range(0.0f, 1.0f));
                            instance.lightmapColor = vegetations[tp].lightColour;
                            float s = UnityEngine.Random.Range(vegetations[tp].minScale, vegetations[tp].maxScale);
                            instance.heightScale = s;
                            instance.widthScale = s;

                            allVegetation.Add(instance);
                            if (allVegetation.Count >= maximumTree) goto TREESDONE;
                        }


                    }
                }
            }
        }
    TREESDONE:
        terrainData.treeInstances = allVegetation.ToArray();

    }

    public void AddNewVegetation()
    {
       vegetations.Add(new Vegetation());
    }

    public void RemoveVegetation()
    {
        List<Vegetation> keptVegetation = new List<Vegetation>();
        for (int i = 0; i < vegetations.Count; i++)
        {
            if (!vegetations[i].remove)
            {
                keptVegetation.Add(vegetations[i]);
            }
        }
        if (keptVegetation.Count == 0) //don't want to keep any
        {
            keptVegetation.Add(vegetations[0]); //add at least 1
        }
        vegetations = keptVegetation;
    }

    public void AddNewSplatHeight()
    {
        splatHeights.Add(new SplatHeights());
    }

    public void RemoveSplatHeight()
    {
        List<SplatHeights> kepSplatHeights = new List<SplatHeights>();
        for (int i = 0; i < splatHeights.Count; i++)
        {
            if (!splatHeights[i].remove)
            {
                kepSplatHeights.Add(splatHeights[i]);
            }
        }
        if (kepSplatHeights.Count == 0) //don't want to keep any
        {
            kepSplatHeights.Add(splatHeights[0]); //add at least 1
        }
        splatHeights = kepSplatHeights;
    }

    float GetSteepness(float[,]heightmap,int x,int y,int width,int height)
    {
        float h = heightmap[x, y];
        int nx = x + 1;
        int ny = y + 1;
        //if on the upper edge of the map find gradient by going backwards
        if(nx>width-1)
        {
            nx = x - 1;
        }
        if(ny>height-1)
        {
            ny = y - 1;
        }
        float dx = heightmap[nx, y] - h;
        float dy = heightmap[x, ny] - h;
        Vector2 gradient = new Vector2(dx, dy);
        float steep = gradient.magnitude;
        return steep;
    }

    public void SplatMaps()
    {
        TerrainLayer[] newSplatPrototype;
        newSplatPrototype = new TerrainLayer[splatHeights.Count];
        int spindex = 0;
        foreach(SplatHeights sh in splatHeights)
        {
            newSplatPrototype[spindex] = new TerrainLayer();
            newSplatPrototype[spindex].diffuseTexture = sh.texture;
            newSplatPrototype[spindex].tileOffset = sh.tileOffset;
            newSplatPrototype[spindex].tileSize = sh.tileSize;
            newSplatPrototype[spindex].diffuseTexture.Apply(true);
            string path = "Assets/TerrainsTextures/New Terrain Layer" + spindex + ".terrainlayer";
            AssetDatabase.CreateAsset(newSplatPrototype[spindex], path);
            spindex++;
            Selection.activeObject = this.gameObject;
        }
        terrainData.terrainLayers = newSplatPrototype;
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        float[,,] splatMapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
        for(int y=0;y<terrainData.alphamapHeight;y++)
        {
            for(int x=0;x<terrainData.alphamapWidth;x++)
            {
                float[] splat = new float[terrainData.alphamapLayers];
                for(int i=0;i<splatHeights.Count;i++)
                {
                    float noise = Mathf.PerlinNoise(x * splatHeights[i].NoiseScale.x, y * splatHeights[i].NoiseScale.y) * splatHeights[i].splatNoiseScaler;
                    float offset = splatHeights[i].offset+ noise;
                    float thisHeightStart = splatHeights[i].minHeight-offset;
                    float thisHeightStop = splatHeights[i].maxHeight+offset;
                    //float steepness = GetSteepness(heightMap, y, x, terrainData.heightmapHeight, terrainData.heightmapWidth);
                    float steepness = terrainData.GetSteepness(y / (float)terrainData.alphamapHeight, x / (float)terrainData.alphamapWidth);
                    if((heightMap[x,y]>=thisHeightStart&&heightMap[x,y]<=thisHeightStop)&&
                        (steepness>=splatHeights[i].Slope.x&&steepness<=splatHeights[i].Slope.y))
                    {
                        splat[i] = 1;
                    }
                }
                NormalizeVector(splat);
                for(int j=0;j<splatHeights.Count;j++)
                {
                    splatMapData[x, y, j] = splat[j];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatMapData);
    }

    void NormalizeVector(float[]v)
    {
        float total = 0;
        for(int i=0;i<v.Length;i++)
        {
            total += v[i];
        }
        for (int i=0;i<v.Length;i++)
        {
            v[i] /= total;
        }
    }

    public void Smooth()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        float smoothProgress = 0;
        EditorUtility.DisplayProgressBar("Smoothing Terrain",
                                 "Progress",
                                 smoothProgress);

        for (int s = 0; s < smoothAmount; s++)
        {
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                for (int x = 0; x < terrainData.heightmapWidth; x++)
                {
                    float avgHeight = heightMap[x, y];
                    List<Vector2> neighbours = GenerateNeighbours(new Vector2(x, y),
                                                                  terrainData.heightmapWidth,
                                                                  terrainData.heightmapHeight);
                    foreach (Vector2 n in neighbours)
                    {
                        avgHeight += heightMap[(int)n.x, (int)n.y];
                    }

                    heightMap[x, y] = avgHeight / ((float)neighbours.Count + 1);
                }
            }
            smoothProgress++;
            EditorUtility.DisplayProgressBar("Smoothing Terrain",
                                             "Progress",
                                             smoothProgress / smoothAmount);

        }
        terrainData.SetHeights(0, 0, heightMap);
        EditorUtility.ClearProgressBar();
    }

    public void MidPointMisplacement()
    {
        float[,] heightMap = GetHeightMap();
        int width = terrainData.heightmapWidth - 1;
        int squareSize = width;
        float heightMin = MPDheightMin;
        float heightMax = MPDheightMax;
        float heightDampener = (float)Mathf.Pow(MPDheightDampenerPower, -1 * MPDroughness);

        int cornerX, cornerY;
        int midX, midY;
        int pmidXL, pmidXR, pmidYU, pmidYD;

        while(squareSize>0)
        {
            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    heightMap[midX, midY] = (float)((heightMap[x, y] + heightMap[cornerX, y] + heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 4.0f +UnityEngine.Random.Range(heightMin,heightMax));
                }
            }
            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    pmidXR = (int)(midX + squareSize);
                    pmidYU = (int)(midY + squareSize);
                    pmidXL= (int)(midX - squareSize);
                    pmidYD= (int)(midY - squareSize);

                    if(pmidXL<=0||pmidYD<=0 || pmidXR >= width-1 || pmidYU >= width-1)
                    {
                        continue;
                    }

                    //calculate the square value for the bottom side
                    heightMap[midX,y]= (float)((heightMap[midX, midY] + heightMap[x, y] + heightMap[midX, pmidYD] + heightMap[cornerX, y]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                    //Calculate the square value for the top side
                    heightMap[midX, cornerY] = (float)((heightMap[x, cornerY] + heightMap[midX, midY] + heightMap[cornerX, cornerY] + heightMap[midX, pmidYU]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                    //Calculate the square value for the left side
                    heightMap[x, midY] = (float)((heightMap[x,y] + heightMap[pmidXL, midY] + heightMap[x, cornerY] + heightMap[midX, midY]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                    //Calculate the square value for the right side
                    heightMap[cornerX, midY] = (float)((heightMap[midX, y] + heightMap[midX, midY] + heightMap[cornerX, cornerY] + heightMap[pmidXR, midY]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                }
            }

            squareSize = (int)(squareSize / 2.0f);
            heightMin *= heightDampener;
            heightMax *= heightDampener;
        }

      

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void Voronoi()
    {
        float[,] heightMap = GetHeightMap();
        for(int p=0;p<voronoiPeaks;p++)
        {
            Vector3 peak = new Vector3(UnityEngine.Random.Range(0, terrainData.heightmapWidth), UnityEngine.Random.Range(voronoiMinHeight,voronoiMaxHeight), UnityEngine.Random.Range(0, terrainData.heightmapHeight));
            if (heightMap[(int)peak.x, (int)peak.z] < peak.y)
            {
                heightMap[(int)peak.x, (int)peak.z] = peak.y;
            }
            else
            {
                continue;
            }
            Vector2 peakLocation = new Vector2(peak.x, peak.z);
            float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapWidth, terrainData.heightmapHeight));
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                for (int x = 0; x < terrainData.heightmapWidth; x++)
                {
                    if (!(x == peak.x && y == peak.z))
                    {
                        float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;
                        float h; 
                        if(voronoiType==VoronoiType.Combined)
                        {
                            h = peak.y - distanceToPeak * voronoiFallOff - Mathf.Pow(distanceToPeak, voronoiDropOff); //combined
                        }
                        else if(voronoiType==VoronoiType.Power)
                        {
                            h = peak.y - Mathf.Pow(distanceToPeak, voronoiDropOff) * voronoiFallOff;//power
                        }
                        else if(voronoiType==VoronoiType.Linear)
                        {
                            h = peak.y - distanceToPeak * voronoiFallOff;//linear
                        }    
                        else if(voronoiType==VoronoiType.SinPow)
                        {
                            h = peak.y - Mathf.Pow(distanceToPeak * 3, voronoiFallOff) - Mathf.Sin(distanceToPeak * 2 * Mathf.PI) / voronoiDropOff;//sinPow
                        }
                        else if (voronoiType == VoronoiType.CosPow)
                        {
                            h = peak.y - Mathf.Pow(distanceToPeak * 2, voronoiFallOff) - Mathf.Cos(distanceToPeak * 2 * Mathf.PI) / voronoiDropOff;//cosPow
                        }
                        else
                        {
                            h = peak.y - Mathf.Sin(distanceToPeak * voronoiDropOff) * voronoiFallOff; //sin
                        }
                        if(heightMap[x,y]<h)
                        {
                            heightMap[x, y] = h;
                        }    
                        
                    }
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void Perlin()
    {
        float[,] heightMap = GetHeightMap();
        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {

                heightMap[x, y] += Utils.FBM((x + perlinOffsetX) * perlinXScale, (y + perlinOffsetY) * perlinYScale, perlinOctaves, perlinPersistance) * perlinHeightScale;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void MultiplePerlinTerrain()
    {
        float[,] heightMap = GetHeightMap();
        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                foreach (PerlinParameters p in perlinParameters)
                {
                    heightMap[x, y] += Utils.FBM((x + p.perlinOffsetX) * p.perlinXScale,
                                                 (y + p.perlinOffsetY) * p.perlinYScale,
                                                    p.perlinOctaves,
                                                    p.perlinPersistance) * p.perlinHeightScale;
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void AddNewPerlin()
    {
        perlinParameters.Add(new PerlinParameters());
    }
    public void RemovePerlin()
    {
        List<PerlinParameters> keptPerlinParameters = new List<PerlinParameters>();
        for(int i=0;i<perlinParameters.Count;i++)
        {
            if(!perlinParameters[i].remove)
            {
                keptPerlinParameters.Add(perlinParameters[i]);
            }
        }
        if(keptPerlinParameters.Count==0) //don't want to keep any
        {
            keptPerlinParameters.Add(perlinParameters[0]); //add at least 1
        }
        perlinParameters = keptPerlinParameters;
    }    

    public void RandomTerrain()
    {
        float[,] heightMap = GetHeightMap();
        for(int x=0;x<terrainData.heightmapWidth;x++)
        {
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                heightMap[x, y] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void LoadTexture()
    {
        float[,] heightMap = GetHeightMap();
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                heightMap[x, y] += heightMapImage.GetPixel((int)(x*heightMapScale.x),(int)(y*heightMapScale.z)).grayscale*heightMapScale.y;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void ResetTerrain()
    {
        float[,] heightMap;
        heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                heightMap[x, y] = 0;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    private void OnEnable()
    {
        Debug.Log("Initialising Terrain Data");
        terrain = this.GetComponent<Terrain>();
        terrainData = Terrain.activeTerrain.terrainData;
    }

    public enum TagType { Tag=0,Layer=1}
    [SerializeField]
    int terrainLayer = -1;
    private void Awake()
    {
        SerializedObject tagManager = new SerializedObject(
                                    AssetDatabase.LoadAllAssetsAtPath("projectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        AddTag(tagsProp, "Terrain",TagType.Tag);
        AddTag(tagsProp, "Cloud", TagType.Tag);
        AddTag(tagsProp, "Shore", TagType.Tag);

        //apply tag changes to tag database
        tagManager.ApplyModifiedProperties();

        SerializedProperty layerProp = tagManager.FindProperty("layers");
        terrainLayer = AddTag(layerProp, "Terrain", TagType.Layer);
        tagManager.ApplyModifiedProperties();

        //take this object
        this.gameObject.tag = "Terrain";
        this.gameObject.layer = terrainLayer;
    }

    int AddTag(SerializedProperty tagsProp,string newTag,TagType tType)
    {
        bool found = false;
        //ensure the tag doesn't already exist
        for(int i=0;i<tagsProp.arraySize;i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(newTag)) { found = true;return i; }

        }
        //add your tag
        if(!found&&tType==TagType.Tag)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = newTag;
        }
        //add a new Layer
        else if(!found &&tType==TagType.Layer)
        {
            for(int j=8;j<tagsProp.arraySize;j++)
            {
                SerializedProperty newLayer = tagsProp.GetArrayElementAtIndex(j);
                //add layer in next empty slot
                if(newLayer.stringValue=="")
                {
                    Debug.Log("Adding New Layer: " + newTag);
                    newLayer.stringValue = newTag;
                    return j;
                }
            }
        }
        return -1;
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

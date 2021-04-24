using System;
using System.IO;
using AMGWebsite.Shared.Enums;
using U_Mod.Enums;
using U_Mod.Extensions;
using U_Mod.Helpers;

namespace U_Mod.Models
{
    public static class IniFileEditor
    {

        public static void EditIniFilesForGame()
        {
            switch (Static.StaticData.CurrentGame)
            {
                case GamesEnum.Oblivion:
                    EditOblivionIniFile();
                    break;

            }
        }

        private static void EditOblivionIniFile()
        {
            //return;

            string iniFileLocation, tempLocation, ini;
            ScreenAspectRatio aspect = SystemHelper.GetScreenAspectRatio();
            GraphicsCard graphics = SystemHelper.GetGraphicsCardType();

            // Oblvion.ini
            ini = "Oblivion.ini";
            iniFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Oblivion", ini);
            tempLocation = Path.Combine(FileHelpers.GetFileExtractionTempFolderPath(), ini);

            if (File.Exists(tempLocation))
                File.Delete(tempLocation);

            using (StreamWriter sw = File.CreateText(tempLocation))
            {
                foreach (var line in File.ReadAllLines(iniFileLocation))
                {
                    string lineText = line switch
                    {
                        { } s when s.ToIniEditString().StartsWith("uGridDistantTreeRange=") => "uGridDistantTreeRange=30",
                        { } s when s.ToIniEditString().StartsWith("uGridDistantCount=") => "uGridDistantCount=50",
                        { } s when s.ToIniEditString().StartsWith("iMultiSample=") => "iMultiSample=0",
                        { } s when s.ToIniEditString().StartsWith("bDoCanopyShadowPass=") && graphics != GraphicsCard.Amd => "bDoCanopyShadowPass=0",
                        { } s when s.ToIniEditString().StartsWith("bDoCanopyShadowPass=") && graphics == GraphicsCard.Amd => "bDoCanopyShadowPass=1",
                        //{ } s when s.ToIniEditString().StartsWith("iSize W=") && aspect == ScreenAspectRatio._4_3 => "iSize W=2048",
                        //{ } s when s.ToIniEditString().StartsWith("iSize H=") && aspect == ScreenAspectRatio._4_3 => "iSize H=1536",
                        //{ } s when s.ToIniEditString().StartsWith("iSize W=") && aspect == ScreenAspectRatio._16_9 => "iSize W=1920",
                        //{ } s when s.ToIniEditString().StartsWith("iSize H=") && aspect == ScreenAspectRatio._16_9 => "iSize H=1080",
                        //{ } s when s.ToIniEditString().StartsWith("iSize W=") && aspect == ScreenAspectRatio._16_10 => "iSize W=1920",
                        //{ } s when s.ToIniEditString().StartsWith("iSize H=") && aspect == ScreenAspectRatio._16_10 => "iSize H=1200",
                        { } s when s.ToIniEditString().StartsWith("fSpecularLOD2=") => "fSpecularLOD2=810.0000",
                        { } s when s.ToIniEditString().StartsWith("fSpecularLOD1=") => "fSpecularLOD1=510.0000",
                        { } s when s.ToIniEditString().StartsWith("iShadowFilter=") => "iShadowFilter=2",
                        { } s when s.ToIniEditString().StartsWith("iActorShadowCountExt=") => "iActorShadowCountExt=5",
                        { } s when s.ToIniEditString().StartsWith("iActorShadowCountInt=") => "iActorShadowCountInt=5",
                        { } s when s.ToIniEditString().StartsWith("bActorSelfShadowing=") => "bActorSelfShadowing=0",
                        { } s when s.ToIniEditString().StartsWith("bShadowsOnGrass=") => "bShadowsOnGrass=0",
                        { } s when s.ToIniEditString().StartsWith("bDynamicWindowReflections=") => "bDynamicWindowReflections=1",
                        { } s when s.ToIniEditString().StartsWith("iTexMipMapSkip=") => "iTexMipMapSkip=0",
                        { } s when s.ToIniEditString().StartsWith("fGrassStartFadeDistance=") => "fGrassStartFadeDistance=6000.0",
                        { } s when s.ToIniEditString().StartsWith("fGrassEndDistance=") => "fGrassEndDistance=7000.0",
                        { } s when s.ToIniEditString().StartsWith("bDecalsOnSkinnedGeometry=") => "bDecalsOnSkinnedGeometry=1",
                        { } s when s.ToIniEditString().StartsWith("fGamma=") => "fGamma=0.7920",
                        { } s when s.ToIniEditString().StartsWith("bAllow30Shaders=") => "bAllow30Shaders=1",
                        { } s when s.ToIniEditString().StartsWith("fNoLODFarDistancePct=") => "fNoLODFarDistancePct=1.0000",
                        { } s when s.ToIniEditString().StartsWith("bUseWaterReflectionsStatics=") => "bUseWaterReflectionsStatics=1",
                        { } s when s.ToIniEditString().StartsWith("bUseWaterReflectionsTrees=") => "bUseWaterReflectionsTrees=1",
                        { } s when s.ToIniEditString().StartsWith("bUseWaterReflections=") => "bUseWaterReflections=1",
                        { } s when s.ToIniEditString().StartsWith("bUseWaterHiRes=") => "bUseWaterHiRes=1",
                        { } s when s.ToIniEditString().StartsWith("bUseWaterDisplacements=") => "bUseWaterDisplacements=0",
                        { } s when s.ToIniEditString().StartsWith("fJumpAnimDelay=") => "fJumpAnimDelay=0.2500",
                        { } s when s.ToIniEditString().StartsWith("fLODTreeMipMapLODBias=") => "fLODTreeMipMapLODBias=-0.5000",
                        { } s when s.ToIniEditString().StartsWith("fLocalTreeMipMapLODBias=") => "fLocalTreeMipMapLODBias=0.0000",
                        { } s when s.ToIniEditString().StartsWith("iPostProcessMillisecondsLoadingQueuedPriority=") => "iPostProcessMillisecondsLoadingQueuedPriority=100",
                        { } s when s.ToIniEditString().StartsWith("iPostProcessMilliseconds=") => "iPostProcessMilliseconds=25",
                        { } s when s.ToIniEditString().StartsWith("bDisplayLODLand=") => "bDisplayLODLand=1",
                        { } s when s.ToIniEditString().StartsWith("bDisplayLODBuildings=") => "bDisplayLODBuildings=1",
                        { } s when s.ToIniEditString().StartsWith("bDisplayLODTrees=") => "bDisplayLODTrees=1",
                        { } s when s.ToIniEditString().StartsWith("fLODFadeOutMultActors=") => "fLODFadeOutMultActors=15.0000",
                        { } s when s.ToIniEditString().StartsWith("fLODFadeOutMultItems=") => "fLODFadeOutMultItems=15.0000",
                        { } s when s.ToIniEditString().StartsWith("fLODFadeOutMultObjects=") => "fLODFadeOutMultObjects=15.0000",
                        { } s when s.ToIniEditString().StartsWith("fLODMultTrees=") => "fLODMultTrees=2.0000",
                        { } s when s.ToIniEditString().StartsWith("fLODMultActors=") => "fLODMultActors=10.0000",
                        { } s when s.ToIniEditString().StartsWith("fLODMultItems=") => "fLODMultItems=10.0000",
                        { } s when s.ToIniEditString().StartsWith("fLODMultObjects=") => "fLODMultObjects=10.0000",
                        { } s when s.ToIniEditString().StartsWith("fGrassEndDistance=") => "fGrassEndDistance=8000.0000",
                        { } s when s.ToIniEditString().StartsWith("fGrassStartFadeDistance=") => "fGrassStartFadeDistance=7000.0000",
                        { } s when s.ToIniEditString().StartsWith("bGrassPointLighting=") => "bGrassPointLighting=1",
                        { } s when s.ToIniEditString().StartsWith("bDoHighDynamicRange=") => "bDoHighDynamicRange=1",
                        { } s when s.ToIniEditString().StartsWith("bUseBlurShader=") => "bUseBlurShader=0",
                        _ => line
                    };

                    sw.WriteLine(lineText);
                }
            }

            File.Delete(iniFileLocation);
            File.Move(tempLocation, iniFileLocation);
           

            // AMD specific
            if (graphics == GraphicsCard.Amd)
            {
                //enbseries.ini
                {
                    ini = "enbseries.ini";
                    iniFileLocation = Path.Combine(FileHelpers.GetGameFolder(), ini);
                    tempLocation = Path.Combine(FileHelpers.GetFileExtractionTempFolderPath(), ini);

                    if (File.Exists(iniFileLocation)) // enbseries is part of non-essential mod so might not exist
                    {
                        if (File.Exists(tempLocation))
                            File.Delete(tempLocation);

                        using (StreamWriter sw = File.CreateText(tempLocation))
                        {
                            foreach (var line in File.ReadAllLines(iniFileLocation))
                            {
                                string lineText = line switch
                                {
                                    { } s when s.StartsWith("EnableDepthOfField") => "EnableDepthOfField=false",
                                    _ => line
                                };

                                sw.WriteLine(lineText);
                            }
                        }

                        File.Delete(iniFileLocation);
                        File.Move(tempLocation, iniFileLocation);
                    }
                }

                //OblivionReloaded.ini
                {
                    ini = "OblivionReloaded.ini";
                    iniFileLocation = Path.Combine(FileHelpers.GetGameFolder(), "Data","OBSE", "Plugins", ini);
                    tempLocation = Path.Combine(FileHelpers.GetFileExtractionTempFolderPath(), ini);

                    if (File.Exists(tempLocation))
                        File.Delete(tempLocation);

                    using (StreamWriter sw = File.CreateText(tempLocation))
                    {
                        foreach (var line in File.ReadAllLines(iniFileLocation))
                        {
                            string lineText = line switch
                            {
                                { } s when s.StartsWith("DepthOfField") => "DepthOfField=0",
                                _ => line
                            };

                            sw.WriteLine(lineText);
                        }
                    }

                    File.Delete(iniFileLocation);
                    File.Move(tempLocation, iniFileLocation);
                }
            }
        }
    }

    
}

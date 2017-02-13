
using SevenZip;

namespace BackBag.Common.Common
{
    public static class SevenZipHelper
    {
        public static void Decompress(string inFile, string outFile)
        {
            SevenZipExtractor.SetLibraryPath(CommonEnvironment.BaseDirectory + "\\7z.dll"); 

            using (var tmp = new SevenZipExtractor(inFile))
            {
                for (int i = 0; i < tmp.ArchiveFileData.Count; i++)
                {
                    tmp.ExtractFiles(outFile, tmp.ArchiveFileData[i].Index);
                }
            }
        }
    }
}

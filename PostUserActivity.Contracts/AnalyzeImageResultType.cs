using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PostUserActivity.Contracts
{
    /// <summary>
    /// Перечень проблем (problemCodeList):
    /// </summary>
    public enum AnalyzeImageResultType
    {

        FaceNotFound = 0,
        FaceMarred = 1,
        FaceBlurred = 2,
        FaceRotated =3,
        СlosedEye = 4,
        PhotoFaceOrLivingPerson = 5,
        PhotoFace = 6,
        MoreThanOnePerson = 7,
        NotFoundSuitableFrame = 9,
        FaceFoundProcessingInProgress = 10,
        InnerException = 11,
        NoError = 12,
        //0 - лицо не найдено;
        //1 - лицо засвечено или низкая контрастность;
        //2 - лицо размыто;
        //3 - лицо сильно повернуто по одной из оси;
        //4 - закрыты оба либо один глаз;
        //5 - идет процесс определения является ли перед камерой фото с лицом или живой человек;
        //6 - перед камерой фото лица;
        //7 - в кадре более одного лица;
        //9 - достигнут предел длительности анализа видео - не найден подходящий кадр.
        //10 - Лицо обнаружено, идет обработка.
        //11  - Внутренняя ошибка библиотеки анализа изображения. 
        NonAcceptablePhoto = 13,
        NonAcceptableScan = 14,
        TryAgainPhoto = 15,
        TryAgainScan = 16,
        ScannerError = 20,
    }

    public static class AnalyzeImageResultDescriptions
    {
        /// <summary>
        /// переводим коды ошибок на понятные человеческие слова
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static List<string> ErrorsDescription(List<AnalyzeImageResultType> errors)
        {
            if (errors == null || !errors.Any())
            {
                return new List<string>();
            }
            return errors.Select(p => ErrorsDict[(int) p]).ToList();            
        }
        
        private static Dictionary<int,string> ErrorsDict = new Dictionary<int, string>()
        {
            { 0, "Лицо не найдено" },
            { 1, "Лицо засвечено или низкая контрастность" },
            { 2, "Лицо размыто" },
            { 3, "Лицо сильно повернуто по одной из осей" },
            { 4, "Закрыт хотя бы один глаз" },
            { 6, "Перед камерой фотография " }, //\n Не удалось детектировать мимику клиента.            
            { 7, "В кадре более одного лица" },            
            { 9, "Достигнут предел длительности \n анализа видео.  \n Подходящий кадр не найден. " },
            { 10, "Лицо обнаружено, идет обработка." },
            { 11, "Внутренняя ошибка библиотеки анализа изображения." },
             { 12, "Без ошибок" },
             {13, "Получено фото недостаточного качества. "}, 
             {14, "Получено фото недостаточного качества. "},
             {15,  "Попробуйте еще раз выполнить фото."},
             {16, "Попробуйте еще раз выполнить сканирование. "},
            //{ 20, "Не удалось отсканировать документ, проверьте подключение к сканеру. В случае повторения ошибки обратитесь в службу технической поддержки." },
            { 20, "Не удалось отсканировать документ." },
        };
    }

}

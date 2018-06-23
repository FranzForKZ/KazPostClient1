/// <summary> 
/// порядок вызова методов
/// 1. при инициализации библиотеки самым первым вызывается метод Init
/// 2. Сканирование:
/// вызывается метод AnalyseScanImage
/// 3. Анализ видео потока. Тк анализ потока происходит покадрово, то
/// 3.1. При старте анализа вызывается метод StartCamAnalyse (очистка буфера, обнуление переменных, счетчиков и тд)
/// 3.2. В цикле вызывается метод AnalyseCamImage, в котором происходит анализ изображения
/// 3.3. По окончанию вызывается метод StopCamAnalyse (очистка буфера, обнуление переменных, счетчиков и тд)    
/// </summary>
public interface IImageAnalyse
{
    #region инициализация параметров анализа изображений

    /// <summary>
    /// передаются параметры для анализа изображения, и отсечения темных, пересвеченных, смазанных изображений
    /// </summary>
    /// <param name="blurDetectionThres"></param>
    /// <param name="blurThreshhold"></param>
    /// <param name="brithnessThresHigh"></param>
    /// <param name="brithnessThresLow"></param>
    /// <param name="lipsThreshhold"></param>
    /// <param name="pitchThreshold"></param>
    /// <param name="rollThreshold"></param>
    /// <param name="yawThreshold"></param>
    /// <param name="yawMean"></param>
    /// <param name="rollMean"></param>
    /// <param name="pitchMean"></param>
    void Init(decimal blurDetectionThres, decimal blurThreshhold, decimal brithnessThresHigh, decimal brithnessThresLow, decimal lipsThreshhold, decimal pitchThreshold, decimal rollThreshold, decimal yawThreshold, decimal yawMean, decimal rollMean, decimal pitchMean);

    #endregion



    #region анализ сканированого изображения

    /// <summary>
    /// анализирует изображение, возвращает структуру
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    ScanResult AnalyseScanImage(Bitmap image);

    #endregion



    #region анализ изображений видео потока

    /// <summary>
    /// вылняется перед запуском передачи кадров
    /// </summary>
    void StartCamAnalyse();

    /// <summary>
    /// анализирует изображение, кадры будут передаваться по 1, внутри должен происходить анализ картинок плюс накопление изображений во внутренний буфер, для анализа движения губ 
    /// (или другого способа определить, что перед камерой живой человек, а не фотография)
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    CamResult AnalyseCamImage(Bitmap image);

    /// <summary>
    /// вызывается, при окончании записи видео, означает, что кадров на обработку передаваться больше не будет, можно очистить буферы
    /// </summary>
    void StopCamAnalyse();

    #endregion
}

/// <summary>
/// структура, которая возвращается после анализа изображения с камеры
/// </summary>
public struct CamResult
{
    /// <summary>
    /// массив прямоугольников с найденными лицами
    /// </summary>
    Rect[] Faces;
    /// <summary>
    /// ошибки в виде int, (битовые флаги, позволяют записать несколько ошибок)
    /// </summary>
    AnalyzeImageResultType Errors;

    /// <summary>
    /// Полученные показатели качества каждого фильтра
    /// </summary>
    ImageDimension[] ImageDimensions;
}

/// <summary>
/// структура, которая возвращается после анализа скана
/// </summary>
public struct ScanResult
{
    /// <summary>
    /// прямоугольник с координатами найденного документа
    /// </summary>
    Rect Document;
    /// <summary>
    /// массив прямоугольников с найденными лицами
    /// </summary>
    Rect[] Faces;
    /// <summary>
    /// ошибки в виде int, (битовые флаги, позволяют записать несколько ошибок)
    /// </summary>
    AnalyzeImageResultType Errors;

    /// <summary>
    /// Полученные показатели качества каждого фильтра
    /// </summary>
    ImageDimension[] ImageDimensions;
}

/// <summary>
/// структура описывающая прямоугольник
/// </summary>
public struct Rect
{
    int X;
    int Y;
    int Width;
    int Height;
}

/// <summary>
/// структура описывающая показатель качества фильтра
/// </summary>
public struct ImageDimension
{
    /// <summary>
    /// название фильтра
    /// </summary>
    string DimensionName;

    /// <summary>
    /// показатель фильтра
    /// </summary>
    string DimensionValue;
}

/// <summary>
/// коды ошибок
/// </summary>
[FlagsAttribute]
public enum AnalyzeImageResultType : int
{
    /// <summary>
    /// Лицо не найдено
    /// </summary>
    FaceNotFound = 0,
    /// <summary>
    /// Лицо засвечено или низкая контрастность.
    /// </summary>
    FaceMarred = 1,
    /// <summary>
    /// Лицо размыто.
    /// </summary>
    FaceBlurred = 2,
    /// <summary>
    /// Лицо сильно повернуто по одной из осей
    /// </summary>
    FaceRotated = 4,
    /// <summary>
    /// Закрыт хотя бы один глаз
    /// </summary>
    СlosedEye = 8,
    /// <summary>
    /// Перед камерой фотография.
    /// </summary>
    NotPerson = 16,
    /// <summary>
    /// В кадре более одного лица
    /// </summary>
    MoreThanOnePerson = 32,
    /// <summary>
    /// Внутренняя ошибка библиотеки обработки видео
    /// </summary>
    InnerException = 64,
}

===========================		Python	===================================================
http://stackoverflow.com/questions/1054849/consuming-python-com-server-from-net
http://stackoverflow.com/questions/1054849/consuming-python-com-server-from-net
http://stackoverflow.com/questions/6624503/call-python-from-net
https://sourceforge.net/projects/pywin32/files/pywin32/Build%20220/


HelloWorld.py - ������� python com object
UnitTest1.cs - ����� ������ Hello �� �������

how to install opencv on local machine 
http://opencv-python-tutroals.readthedocs.io/en/latest/py_tutorials/py_setup/py_setup_in_windows/py_setup_in_windows.html

anaconda

��� ������� ����� ���������: dlib, OpenCV, numpy, sklearn, cPickle,scipy
conda install numpy=1.10.4
conda install scikit-learn=0.17
conda install -c anaconda cloudpickle=0.2.1
conda install -c menpo opencv3=3.1.0
conda install -c menpo dlib=18.18 (������ pip install dlib ��� ������� https://pypi.python.org/pypi/dlib/18.17.100)
��� windows xp embdded ������������� ������ ��������� ���������������� ��������� ������:
http://www.lfd.uci.edu/~gohlke/pythonlibs/
https://pypi.python.org/pypi/dlib/18.17.100   - ������ dlib

PATH
C:\install\anaconda;C:\install\anaconda\Scripts;C:\install\anaconda\Library\bin

�������� ���� � ����� frame_quality_analyser.py
PYTHON_SRC_PATH = 
�� ����� � PythonSrc
('d:/!!Projects/TechnoServ/SrcCs/PythonSrc/')

then
in windows command promt:
PythonSrc>frame_quality_analyser_com.py --register


after COM exception  80040154 should help:
http://stackoverflow.com/questions/7197506/how-to-repair-comexception-error-80040154

===========================		Interactive Services	===================================================
http://stackoverflow.com/questions/3351531/how-to-set-interact-with-desktop-in-windows-service-installer
http://stackoverflow.com/questions/4237225/windows-service-allow-service-to-interact-with-desktop/4237283#4237283
http://www.codeproject.com/Articles/35773/Subverting-Vista-UAC-in-Both-and-bit-Archite
https://msdn.microsoft.com/en-us/library/ms683502(VS.85).aspx
https://social.msdn.microsoft.com/Forums/vstudio/en-US/93b084d7-f7e9-49b9-8f88-b8f8e3c49aa5/wmi-to-change-window-service-property?forum=csharpgeneral
http://stackoverflow.com/questions/5559419/how-to-programmatically-change-settings-for-wmi-instead-of-using-wmimgmt-msc-sna
launch the application
ApplicationLoader.PROCESS_INFORMATION procInfo;
ApplicationLoader.StartProcessAndBypassUAC(applicationName, out procInfo);



Segoe UI

http://stackoverflow.com/questions/8039937/preloading-whole-main-form-while-users-are-filling-login-textbox
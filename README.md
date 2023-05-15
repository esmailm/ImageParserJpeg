# Service Notes

- Imagination service is developed based on SixLabors library.

- System.drawing is fit for windows apps only and will not 
  work with another OS like linux.

- The service is receiving any type of data, but it has two phases 
  of validation first validate file size and second validate input 
  image formats that can be loaded with SixLabors .

- The service will adjust the input image frame size to the target number of 
  pixels if it larger.

- The transformImage is not affecting the image width and height ratio.

- According to the target image file size the quality is optimized to 
  the nearest file size result.

- Optimizing file size developed to compromise between the desired 
  file size with tolerance versus image quality binary search.

- The global exception handler is applied with logs for each error message.



## Service variables

The service has some environment variables to control all processes for each request like 
MaxInputImageSize and MinInputImageSize control the upcoming file size and image frame 
resizing according to number of pixels form multiplication TargetPixelsWidth and 
TargetPixelsHeight and it keeps the original frame size if it smaller.
The TargetImageCompress and TargetImageCompressTolerance controls the target output file size 
and the TargetImageCompressTolerance variable make the accepted image size tolerance can be wider.


# Image Conversion Service

The goal of this project is to implement a Web API that converts
common web image file formats (e.g., PNG) into JPEG.

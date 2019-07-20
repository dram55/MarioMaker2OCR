# Mario Maker 2 OCR
This program will capture level information & events from a Mario Maker 2 game feed and display on a locally hosted web app. 

### [Video Instructions - https://youtu.be/HsQZIANz1Yc](https://youtu.be/HsQZIANz1Yc)

## OCR Program
![](screenshots/screenshot.jpg) ![](screenshots/preview.jpg)

## Web Overlays
![](screenshots/smmbar.png) 

![](screenshots/drambar.png)



## How To Use
### Setup
- If you are using OBS, download the [OBS-VirtualCam](https://obsproject.com/forum/resources/obs-virtualcam.539/) plugin.
    - Needed because this program can't access a capture card already in use by OBS.
- In OBS, put an 'Effect' on your capture device, select the 'VirtualCam' effect.
- Be sure to press the 'Start' button on the VirtualCam.


### Use
1) Open OBS before this program to avoid conflict.
1) In OBS, verify the VirtualCam is running. 
1) Open the Mario Maker 2 OCR program.
1) Select the capture device with Mario Maker 2 gameplay from the dropdown (use OBS VirtualCam from setup).
1) Select a port number to host the web server.
1) Select the resolution.
1) Press **Start** button.
1) The bottom status strip is updated.
   - A green box indicates the program is running. 
   - Link will allow you to look at the web overlays, which display current level data.
1) Setup a Browser source in OBS to point to the web overlay you would like.
    - ex: http://localhost:3000/smmbar
    - ex: http://localhost:3000/drambar
    - Select option to have the source refresh every time it is displayed.
1) Play Mario Maker 2...
1) When an event is detected:
   - The level information is written to `ocrLevel.json` file. 
   - The web overlays will updated.


## Technical

1) Every 250ms the program reads a frame from the video feed.
1) This frame will be added to a buffer containing the last 10 frames. 
1) If a black screen is detected:
   1) Check the buffer for a level screen.
      - Run the reference image and current frame through [`OpenCv.AbsDiff()`](http://www.emgu.com/wiki/files/4.1.0/document/html/17b44e5d-44c9-9cc5-1418-e17c0ff64e3f.htm)
      - Take the resultant image and run it through a [`OpenCv.ThresholdBinary()`](http://www.emgu.com/wiki/files/4.1.0/document/html/046038a6-4e0e-ba4c-d01f-eb3fc1c29383.htm) filter.
      - Take the resultant image and count the non-blank pixels.
      - Divide the non-blank pixels by the total pixels in the frame.
      - Subtract this value from 1 to get the percentage the images match.
      + If the images are at least a 94% match:
         - Grab the level author, level code & level name screeb regions from the current frame based on predefined rectangles.
         - Run each of these through the [`OpenCv.GammaCorrect(2.5)`](http://www.emgu.com/wiki/files/4.1.0/document/html/c95af2fa-a121-374e-8b72-d657aa162d1a.htm) and [`OpenCv.Resize(2.8)`](http://www.emgu.com/wiki/files/4.1.0/document/html/bf5b6c7e-193a-c469-026b-3e8fdf2f2306.htm) filters for better contrast and size. This gives better results for the OCR.
         - Pass these images to the Tesseract library to perform the OCR.
            - For the level code, use the whitelist command to help with accuracy: `tessedit_char_whitelist ABCDEFGHJKLMNPQRSTUVWXYZ0123456789-`
         - Write the results to a JSON output file.
         - Broadcast new level info thru the Websockets server.
   1) For each image frame in the buffer:
      - For each available template image: (death icons, exit level button, start over button)
         - Run the frame and template image through [`OpenCv.MinMax()`](http://www.emgu.com/wiki/files/4.1.0/document/html/6d14cf3b-8946-cc94-924b-e2b1fe12e5e2.htm).
         - If there is a match:
            - Broadcast this event thru the Websockets server.
            - Break from logic.

### Create your own overlays
In the `\web` directory you will see 2 overlays to use/modify as an example. This directory is where the web server will be hosted, any web apps created here can be used as a stream overlay. 

The apps can read in the broadcasted websocket messages listed below and display the information as desired.

![](screenshots/web.jpg)

### Websocket Message API
``` JSON
On new level
{
  "level": {
    "author": "Valdio",
    "code": "8DY-1WC-FQG",
    "name": "Thwomping Grounds"
  }
}

On death
{ "type" : "death" }

On start over
{ "type" : "restart" }

On exit
{ "type" : "exit" }

On gameover
{ "type" : "gameover" }
```


### Dependecies
The [OpenCV](https://opencv.org/) library is used for image processing and [Tesseract](https://opensource.google.com/projects/tesseract) library is used for OCR (Optical Character Recognition). [EmguCV](http://www.emgu.com/wiki/index.php/Main_Page) provides a .NET wrapper for both of these libraries and is directly used for this project. [EmbedIO](https://github.com/unosquare/embedio) for Websocket and HTTP server.

### NuGet Packages
- DirectShowLib v1.0.0
- EMGU CV v4.1.0.3420
- Newtonsoft.Json v12.0.2
- log4net v2.0.8
- EmbedIO v2.9.2

## Contribution
- [dram55](https://twitter.com/dram555) - Original OCR program to read level info to a file.
- zi - (zi#7981 on discord) - Added preview window, websocket/http server broadcasting for all events, warp bar style html page, and various fixes/performance improvements. 
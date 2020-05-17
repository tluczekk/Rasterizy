# Rasterizy

## General Description
**Rasterizy** is a program which purpose is to create and manipulate geometric shapes using vector approach to graphics. 
Available shapes are as follows:
- Line
- Circle
- Polygon
- Capsule
- Rectangle

For each shape there are available several actions:
- Draw
- Move
- Delete
- Thicken
- Recolor
- Move vertex (for **Polygon** and **Rectangle** only)
- Clip (**Polygon** only)
- Fill (**Polygon** only, either with provided image or with a solid colour)
> **Note:**  **Capsule** is not fully implemented and can only be drawn.

Also, antialiasing can be turned on for all the shapes (which will result in reverting thickness parameter to 1), 

## Requirements
Executable file of the program can be run on  a Windows operating system, or with an appropriate software on other OS's, which would let user run **.exe** files.

## Manual
Upon entering any mode after clicking a button, an appropriate message is displayed in a label. 
### Drawing
For all shapes, **Draw** draws shapes with thickness specified in **Thicc** field and color chosen from the **Color** dialog. 
#### Line
In order to draw a line, one has to turn the **Line** mode on, make sure that **Draw** attribute is selected and click twice on the picturebox, selecting two endpoints of the line, which is drawn onto the picturebox using **DDA** algorithm.
#### Circle
To draw a circle, one has to turn the **Circle** mode with **Draw** attribute, and select two points - first one is going to be the center, second one - a radius. Circle is drawn using an **Alternative Midpoint Circle** algorithm.
#### Polygon
To be able to draw a polygon, one ought to select the **Poly** mode with **Draw** attribute, and then pick multiple points on the picturebox. For polygon to be closed, one has to select a final pixel in an immediate vicinity of the first one - then, program will automaticaly close the polygon and draw it onto the picturebox. To make matters easier, starting vertex is shown as a single black pixel, so user can easily come back to it, should he or she consider that polygon is to be closed. 
#### Capsule
In order to draw a capsule, **Capsule** mode with **Draw** attribute has to be selected. Then, user has to pick three points, which are going to be a symmetric center of a capsule, center of one circle and radius of it, respectively.
#### Rectangle 
To draw a rectangle, **Rectangle** mode with **Draw** attribute should be chosen. Then, two picked points will make a diagonal of rectangle, which will be put onto the picturebox (rectangle, not diagonal).
### Move
To move any shape, one has to select an appropriate mode with **Move** attribute. Then, user has to pick two points - one on the perimeter of a given shape, and second wherever on the picturebox user wants this point to be after translation. Then, these two points will make up vector, which every point of the shape is to be translated by.
### Delete
In order to delete a shape, user ought to select an appropriate mode with **Delete** attribute. One click on the perimeter of a given shape, will remove this shape from the picturebox.
### Thicken
To be able to change a thickness of a given shape, one has to select an appropriate mode with **Thicken** attribute, and change the **Thicc** field to a desired value. One click on the perimeter of a given shape, will change its thickness to a value specified in **Thicc** field. Lines are thickened using **Copying pixels** algorithm.
>**Note:** Due to the different method of implementation. thickening and drawing **Circle** and **Capsule** with a thickness greater than 1 will take more time than similar operation performed on the different shapes
### Recolor
To change a colour of a given shape, one has to select an appropriate mode with **Recolor** attribute, and change the colour of **Color** dialog to a preferred one. One click on the perimeter of a given shape, will change its colour to a value specified in **Color** dialog.
### Move vertex
In order to move a vertex of a given polygon, one has to select **Poly** or **Rectangle** mode with **Move vertex** attribute. Then, user has to select a point in a close vicinity of a vertex of any polygon, and then select a new point where this vertex is to be put. In **Polygon** edges leading to the moved vertex will be corrected, while in **Rectangle** all edges and two neighbour vertices are going to be adjusted.
### Clip
To clip a polygon to a second one, user has to select **Poly** mode with **Clip** attribute. Then, one draws two polygons, one after another, and the first one is clipped to the second one using **Sutherland-Hodgman** algorithm. After operation is performed, these two polygons are subject to any Polygon operation.  
### Antialiasing
Clicking the **Antialiasing** button will turn on or off the antialiasing, implemented using **Xiaolin Wu** algorithm. When antialiasing is turned on, all thicknesses are reduced to 1, but after turning the antialiasing off, they come back to their previous values.
### Filling
To fill a **Polygon**, one has to select one of two options (both use **Active Edge Table** algorithm with **Edge Table** method of updating):
#### Filling with a solid colour
User has to click **Fill poly** button, and then select a polygon to fill with a colour selected in a **Color** dialog
#### Filling with an image
One ought to click **Fill with Image** button, which will open a file dialog to choose an image from disk. Then, user has to select a **Polygon** to fill with the chosen image.
### Saving and Loading
User can save and load contents of the picturebox to and from a **.minicg** file using **Save** and **Load** dialogs.
### Clearing the picturebox
To clear a picturebox, one has to click **Clear** button which will remove all the shapes and information associated with them 

##### Jakub Tłuczek 2020

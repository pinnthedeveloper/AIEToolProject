﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AIEToolProject.Source;

namespace AIEToolProject
{

    public partial class EditorForm : Form
    {
        //string that the file loaded from
        public string loadedPath = "";

        //container of objects to update
        public List<BaseObject> objects;

        //container for event listers that exclusively recieve the events, all event listers
        //recieve events if the list is empty
        public List<EventListener> exclusives;

        //empty buttons that define the scrollable area
        public Button topLeft = null;
        public Button bottomRight = null;

        //remembers the last valid scroll position
        public Point safeScrollPosition = new Point(0, 0);

        //amount of padding on the scrollable area
        public int windowPadding = 300;

        //type of node to spawn when requested
        public NodeType spawnType = NodeType.ACTION;

        /*
        * public EditorForm()
        * constructor, assigns default values
        */
        public EditorForm()
        {
            InitializeComponent();

            objects = new List<BaseObject>();
            exclusives = new List<EventListener>();

            BaseObject obj = new BaseObject();

            NodeSpawner spawnerComp = new NodeSpawner();

            spawnerComp.form = this;

            EventListener listenerComp = new EventListener();

            listenerComp.mousePressed = spawnerComp.MousePressedCallback;
            listenerComp.mouseReleased = spawnerComp.MouseReleasedCallback;

            listenerComp.container = obj;
            spawnerComp.container = obj;

            obj.components.Add(listenerComp as EventListener);
            obj.components.Add(spawnerComp as NodeSpawner);

            objects.Add(obj);

        }


        /*
        * SetScrollableArea
        * 
        * iterates through every node, setting a rectangle
        * (topLeft and bottomRight) that surrounds all nodes
        *
        * @param int padding - the padding of the rectangle that surrounds all nodes
        * @returns void
        */
        public void SetScrollableArea(int padding)
        {

            Point scrollPos = new Point(this.HorizontalScroll.Value, this.VerticalScroll.Value);

            //track the best position for the rectangle as the list of nodes is iterated through
            Point bottomRightBest = new Point(0, 0);

            //define a list of nodes to check
            List<Node> nodes = new List<Node>();

            //get the size of the objects list
            int objSize = objects.Count;

            //iterate through all of the objects
            for (int i = 0; i < objSize; i++)
            {
                //store in a temp value for performance and readability
                BaseObject obj = objects[i];

                //get the size of the object's components list
                int compSize = obj.components.Count;

                //iterate through all of the components in the object
                for (int j = 0; j < compSize; j++)
                {
                    //store in a temp valu efor performance and readability
                    BaseComponent comp = obj.components[j];

                    //check that
                    if (comp is Node)
                    {
                        nodes.Add(comp as Node);
                    }
                }
            }

            //default setting
            if (nodes.Count > 0)
            {
                bottomRightBest = new Point((int)nodes[0].collider.x, (int)nodes[0].collider.y);
            }

            //iterate through all nodes in the nodes container
            foreach (Node n in nodes)
            {
                //move the left limit if it too close
                if (bottomRightBest.X < (int)n.collider.x)
                {
                    bottomRightBest = new Point((int)n.collider.x, bottomRightBest.Y);
                }

                //move the right limit if it too close
                if (bottomRightBest.Y < (int)n.collider.y)
                {
                    bottomRightBest = new Point(bottomRightBest.X, (int)n.collider.y);
                }
            }

            bottomRightBest.X += padding;
            bottomRightBest.Y += padding;

            //set the scrollbars based on the scrollable area

            //test that the scrollbar is required
            if (this.Width > bottomRightBest.X)
            {
                this.hScrollBar.Visible = false;
            }
            else
            {
                this.hScrollBar.Visible = true;
                this.hScrollBar.Minimum = 0;
                this.hScrollBar.Maximum = bottomRightBest.X;
                this.hScrollBar.LargeChange = this.Width;
            }

            //test that the scrollbar is required
            if (this.Height > bottomRightBest.Y)
            {
                this.vScrollBar.Visible = false;
            }
            else
            {
                this.vScrollBar.Visible = true;
                this.vScrollBar.Minimum = 0;
                this.vScrollBar.Maximum = bottomRightBest.Y;
                this.vScrollBar.LargeChange = this.Height;
            }

            //maximum values that the scroll bars can have
            int maxX = (int)(bottomRightBest.X - this.Width);
            int maxY = (int)(bottomRightBest.Y - this.Height);

            //clamp the scrolling values
            if (this.hScrollBar.Value > maxX)
            {
                //minimum value is 0
                if (maxX < 0)
                {
                    this.hScrollBar.Value = 0;
                }
                else
                {
                    this.hScrollBar.Value = maxX;
                }
            }

            if (this.vScrollBar.Value > maxY)
            {

                //minimum value is 0
                if (maxY < 0)
                {
                    this.vScrollBar.Value = 0;
                }
                else
                {
                    this.vScrollBar.Value = maxY;
                }
            }

            safeScrollPosition = new Point(this.hScrollBar.Value, this.vScrollBar.Value);
        }


        /*
        * TriggerCallback
        * 
        * callback when the form is clicked on (down)
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @param CallbackType t - the type of callback to send
        * @returns void
        */
        private void TriggerCallback(object sender, EventArgs e, CallbackType t)
        {
            //get the size of the object list
            int objSize = objects.Count;

            //iterate through the objects, updating information
            for (int i = 0; i < objSize; i++)
            {
                //store in a temp value for readability and performance
                BaseObject obj = objects[i];

                //get the size of the object's component list
                int compSize = obj.components.Count;

                //iterate through all components in the object, checking if each is an event listener
                for (int j = 0; j < obj.components.Count; j++)
                {
                    //store the base value
                    BaseComponent comp = obj.components[j];

                    //check if the component is an event listener
                    if (comp is EventListener)
                    {
                        //cast the base component to it's true type
                        EventListener eventListener = comp as EventListener;

                        //update the correct information
                        if (e is MouseEventArgs)
                        {
                            eventListener.mouseEventArgs = e as MouseEventArgs;
                        }
                    }
                }
            }

            //get the size of the exclusive listeners list
            int size = exclusives.Count;

            if (size == 0)
            {
                //iterate through all objects, sending all listeners the event
                for (int i = 0; i < objects.Count; i++)
                {
                    //store in a temp value for readability and performance
                    BaseObject obj = objects[i];

                    //get the size of the object's component list
                    int compSize = obj.components.Count;

                    //iterate through all components in the object, checking if each is an event listener
                    for (int j = 0; j < compSize; j++)
                    {
                        //store the base value
                        BaseComponent comp = obj.components[j];

                        //check if the component is an event listener
                        if (comp is EventListener)
                        {
                            //cast the base component to it's true type
                            EventListener eventListener = comp as EventListener;

                            int prevSize = objects.Count;

                            //call the requested type of callback
                            switch (t)
                            {
                                case CallbackType.MOUSE_PRESSED: eventListener.mousePressed(eventListener); break;
                                case CallbackType.MOUSE_RELEASED: eventListener.mouseReleased(eventListener); break;
                                case CallbackType.MOUSE_MOVED: eventListener.mouseMoved(eventListener); break;
                            }

                            //reverse the iterator effect to avoid skipping over items when one is deleted
                            if (objects.Count < prevSize)
                            {
                                i--;
                            }
                        }
                    }

                }
            }
            else
            {
                //iterate through all the exclusive listeners, sending them the event
                for (int i = 0; i < size; i++)
                {
                    //store in a temp value for performance and readability
                    EventListener eventListener = exclusives[i];

                    //call the requested type of callback
                    switch (t)
                    {
                        case CallbackType.MOUSE_PRESSED: eventListener.mousePressed(eventListener); break;
                        case CallbackType.MOUSE_RELEASED: eventListener.mouseReleased(eventListener); break;
                        case CallbackType.MOUSE_MOVED: eventListener.mouseMoved(eventListener); break;
                    }
                }
            }

            SetScrollableArea(windowPadding);

            //force the form to re-draw itself
            Invalidate();
            this.Refresh();
        }


        /*
        * EditorForm_MouseDown
        * 
        * callback when the form is clicked on (down)
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void EditorForm_MouseDown(object sender, EventArgs e)
        {
            TriggerCallback(sender, e, CallbackType.MOUSE_PRESSED);
        }


        /*
        * EditorForm_MouseUp 
        * 
        * callback when the form is released (up)
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void EditorForm_MouseUp(object sender, MouseEventArgs e)
        {
            TriggerCallback(sender, e, CallbackType.MOUSE_RELEASED);
        }


        /*
        * EditorForm_MouseMove 
        * 
        * callback when the mouse moves over the form
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void EditorForm_MouseMove(object sender, MouseEventArgs e)
        {
            TriggerCallback(sender, e, CallbackType.MOUSE_MOVED);
        }


        /*
        * EditorForm_Click 
        * 
        * callback when the form is clicked (down adn then up)
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void EditorForm_Click(object sender, EventArgs e)
        {

        }


        /*
        * OffsetRectangle 
        * 
        * subtracts the relative vector formed by "point"
        * from the rectangle and forms a new rectangle
        * 
        * @param Rectangle local - the rectangle to offset
        * @param Point point - the offset to apply
        * 
        */
        public Rectangle OffsetRectangle(Rectangle local, Point point)
        {
            //create a new rectangle
            Rectangle transformed = new Rectangle();

            transformed.Location = new Point(local.Location.X - point.X, local.Location.Y - point.Y);
            transformed.Size = local.Size;

            return transformed;

        }


        /*
        * OffsetPolygon 
        * 
        * subtracts the relative vector formed by "point"
        * from the polygon and forms a new polygon
        * 
        * @param Point[] local - the polygon (list of points that builds it) to offset
        * @param Point point - the offset to apply
        * 
        */
        public Point[] OffsetPolygon(Point[] local, Point point)
        {
            int size = local.Count();
            
            //new array of transformed points
            Point[] transformed = new Point[size];

            //iterate through all of the points, applying the offset to all of them
            for (int i = 0; i < size; i++)
            {
                transformed[i] = new Point(local[i].X - point.X, local[i].Y - point.Y);
            }

            return transformed;

        }


        /*
        * OnPaint
        * protects Form's OnPaint(PaintEventArgs e)
        * 
        * the custom draw call of the form, draws all nodes
        * along with their text and connections
        * 
        * @param PaintEventArgs e - the arguments provided to paint the custom textures
        * @returns void
        */
        protected override void OnPaint(PaintEventArgs e)
        {

            //called the hidden function
            base.OnPaint(e);

            SetScrollableArea(windowPadding);

            //get the graphics object to paint with
            Graphics g = e.Graphics;

            //get the size of the objects list
            int objSize = objects.Count;

            //iterate through all of the objects, pre-rendering each
            for (int i = objSize - 1; i >= 0; i--)
            {
                //store in a temp value for readability and performance
                BaseObject obj = objects[i];

                //get the size of the object's component list
                int compSize = obj.components.Count;

                //iterate through all components in the object, checking if each is a node renderer
                for (int j = 0; j < obj.components.Count; j++)
                {
                    //store the base value
                    BaseComponent comp = obj.components[j];

                    //check if the component is an event listener
                    if (comp is NodeRenderer)
                    {
                        //cast the base object to it's true type
                        NodeRenderer renderer = comp as NodeRenderer;

                        renderer.PreRender(this, g);
                    }
                }
            }

            //iterate through all of the objects, rendering each
            for (int i = objSize -1 ; i >= 0; i--)
            {
                //store in a temp value for readability and performance
                BaseObject obj = objects[i];

                //get the size of the object's component list
                int compSize = obj.components.Count;

                //iterate through all components in the object, checking if each is a node renderer
                for (int j = 0; j < obj.components.Count; j++)
                {
                    //store the base value
                    BaseComponent comp = obj.components[j];

                    //check if the component is an event listener
                    if (comp is NodeRenderer)
                    {
                        //cast the base object to it's true type
                        NodeRenderer renderer = comp as NodeRenderer;

                        renderer.Render(this, g);
                    }
                }
            }

        }


        private void EditorForm_Load(object sender, EventArgs e)
        {

        }



        private void EditorForm_Resize(object sender, EventArgs e)
        {
            SetScrollableArea(windowPadding);

            //force the form to re-draw itself
            Invalidate();          
            this.Refresh();
        }


        /*
        * hScrollBar_Scroll 
        * 
        * callback when the horizontal scroll bar is used
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            safeScrollPosition = new Point(this.hScrollBar.Value, safeScrollPosition.Y);

            SetScrollableArea(windowPadding);

            //force the form to re-draw itself
            Invalidate();
            this.Refresh();
        }


        /*
        * vScrollBar_Scroll 
        * 
        * callback when the vertical scroll bar is used
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            safeScrollPosition = new Point(safeScrollPosition.X, this.vScrollBar.Value);

            SetScrollableArea(windowPadding);

            //force the form to re-draw itself
            Invalidate();
            this.Refresh();
        }
    }
}

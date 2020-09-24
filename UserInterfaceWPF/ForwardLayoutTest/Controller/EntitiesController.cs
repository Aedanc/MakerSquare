using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Entities;
using MakerSquare.FrontFacingECS;

namespace ForwardLayoutTest
{
    public class EntitiesController : IEquatable<EntitiesController>
    {
        /// <summary>
        /// Generate ID for the entities on the canvas
        /// </summary>
        public class IDGenerator
        {
            private int _id = 0;

            public int NewID()
            {
                _id++;
                return _id;
            }

            public void Update()
            {
                _id++;
            }
        }

        /// <summary>
        /// Entities that is on the canvas
        /// </summary>
        private Dictionary<string, Entity> canvasEntities;

        /// <summary>
        /// For the ID of the Entity of the canvas Entities
        /// </summary>
        public IDGenerator generator;

        /// <summary>
        /// constructor
        /// </summary>
        public EntitiesController()
        {
            this.canvasEntities = new Dictionary<string, Entity>();
            generator = new IDGenerator();
        }

        //////////////////////////
        /////// Find Method //////
        //////////////////////////

        public Entity FindCanvasEntityByStringId(string id)
        {
            foreach (Entity entity in canvasEntities.Values)
            {
                if (entity.canvas_image_data.image_id == id)
                {
                    return (Entity)entity;
                }
            }
            return null;
        }

        //////////////////////////
        ///// tools methods //////
        //////////////////////////

        /// <summary>
        /// Add a entity in canvasEntity
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddCanvasEntity(String key, Entity value)
        {
            foreach (String _key in canvasEntities.Keys)
                if (_key == key)
                    return;
            canvasEntities.Add(key, value);
        }

        /// <summary>
        /// return all the values in Canvas entities
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entity> CanvasValues()
        {
            return canvasEntities.Values;
        }

        /// <summary>
        /// Equals with object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>bool: true if equal, false if not</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as EntitiesController);
        }

        /// <summary>
        /// Count the nummber of entities in canvasEntities
        /// </summary>
        /// <returns>int: the number of entities</returns>
        public int CountCanvasEntities()
        {
            return canvasEntities.Count();
        }
        /// <summary>
        /// Equals with EntitiesController
        /// </summary>
        /// <param name="other"></param>
        /// <returns>bool: true if equal, false if not</returns>
        public bool Equals(EntitiesController other)
        {
            return other != null &&
                   EqualityComparer<Dictionary<string, Entity>>.Default.Equals(canvasEntities, other.canvasEntities) &&
                   EqualityComparer<IDGenerator>.Default.Equals(generator, other.generator);
        }

        /// <summary>
        /// Unselect all entities in "selected" state
        /// </summary>
        public void UnselectAll()
        {
            foreach (Entity canvasEntity in canvasEntities.Values)
            {
                if (canvasEntity is Entity && canvasEntity.Selected())
                    canvasEntity.Unselect();
            }
        }

        /// <summary>
        /// Returns the number of actually selected entities
        /// </summary>
        /// <returns></returns>
        public int CountSelectedEntities()
        {
            int total = 0;

            foreach (Entity canvasEntity in canvasEntities.Values)
            {
                if (canvasEntity is Entity && canvasEntity.Selected())
                    total++;
            }

            return total;
        }

        /// <summary>
        /// Returns all the currently selected entities
        /// </summary>
        /// <returns></returns>
        public List<Entity> GetSelectedEntities()
        {
            List<Entity> selectedEntities = new List<Entity>();

            foreach (Entity canvasEntity in canvasEntities.Values)
            {
                if (canvasEntity.Selected())
                    selectedEntities.Add(canvasEntity);
            }

            return (selectedEntities.Any() ? selectedEntities : null);
        }

        public void DeleteEntity(Entity entity)
        {
            var entity_pair = canvasEntities.Where(f => f.Key == entity.Name).First();
            
            entity.DeleteFromCanvas();
            canvasEntities.Remove(entity_pair.Key);
        }

        public void DeleteSelectedEntities()
        {
            var itemsToRemove = canvasEntities.Where(f => f.Value is Entity && f.Value.Selected()).ToArray();

            foreach (var item in itemsToRemove)
            {
                item.Value.DeleteFromCanvas();
                canvasEntities.Remove(item.Key);
            }
        }

        public void SelectEntitiesInZone(Rectangle selectBox)
        {
            Point topLeft = new Point(Canvas.GetLeft(selectBox), Canvas.GetTop(selectBox));
            Point bottomRight = new Point(topLeft.X + selectBox.Width, topLeft.Y + selectBox.Height);

            foreach (Entity entity in canvasEntities.Values)
            {
                Point entityPos = entity.GetPosition();

                if (topLeft.X <= entityPos.X && entityPos.X <= bottomRight.X &&
                    topLeft.Y <= entityPos.Y && entityPos.Y <= bottomRight.Y)
                    entity.Select();
            }
        }

        /// <summary>
        /// Get the hashCode
        /// </summary>
        /// <returns>int: hashCode </returns>
        public override int GetHashCode()
        {
            var hashCode = 1651148459;
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, Entity>>.Default.GetHashCode(canvasEntities);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDGenerator>.Default.GetHashCode(generator);
            return hashCode;
        }

        /// <summary>
        /// operator ==
        /// </summary>
        /// <param name="controller1"></param>
        /// <param name="controller2"></param>
        /// <returns>bool: true if equal, false if not</returns>
        public static bool operator ==(EntitiesController controller1, EntitiesController controller2)
        {
            return EqualityComparer<EntitiesController>.Default.Equals(controller1, controller2);
        }

        /// <summary>
        /// operator !=
        /// </summary>
        /// <param name="controller1"></param>
        /// <param name="controller2"></param>
        /// <returns>bool: false if equal, true if not</returns>
        public static bool operator !=(EntitiesController controller1, EntitiesController controller2)
        {
            return !(controller1 == controller2);
        }

        /// <summary>
        /// use this method before codegen to write components data in entities
        /// </summary>
        public void SaveEntitiesData()
        {
            foreach (Entity entity in canvasEntities.Values)
            {
                entity.Components.Clear();
                foreach (var component in entity.GetComponents())
                {
                    component.FillData();
                    entity.Components.AddRange(component.engineData);
                }
            }
        }

        /// <summary>
        /// Logs canvas entities components
        /// </summary>
        public void EntitiesComponentsLog()
        {
            foreach (Entity entity in canvasEntities.Values)
            {
                foreach (var component in entity.Components)
                {
                    Console.WriteLine("component: " + component.ToString());
                }
            }
        }

        /// <summary>
        /// CanvasEntities getter
        /// </summary>
        /// <returns></returns>
        public List<FFEntity> GetCanvasEntities()
        {
            List<FFEntity> ffentities = new List<FFEntity>();

            foreach(var entity in canvasEntities.Values)
            {
                ffentities.Add(entity);
            }
            return ffentities;
        }
    }
}

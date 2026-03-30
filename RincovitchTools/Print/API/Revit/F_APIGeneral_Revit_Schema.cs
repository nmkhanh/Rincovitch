using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Newtonsoft.Json.Linq;
using Revit.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RincovitchTools.Print.API.Revit
{
  public class F_APIGeneral_Revit_Schema
  {
    public static void CreateSchema(string guid, List<string> name_para)
    {
      try
      {
        Guid schemaGuid = new Guid(guid);

        // 1?? L?y schema n?u dã t?n t?i
        Schema schema = Schema.Lookup(schemaGuid);

        if (schema == null)
        {
          SchemaBuilder sb1 = new SchemaBuilder(new Guid(guid));
          sb1.SetReadAccessLevel(AccessLevel.Public);
          sb1.SetWriteAccessLevel(AccessLevel.Public);
          sb1.SetVendorId("NMKhanh");
          foreach (var item in name_para)
          {
            sb1.SetSchemaName(item);
            FieldBuilder fieldUser = sb1.AddSimpleField(item, typeof(string));
            fieldUser.SetDocumentation("Set");
          }
          schema = sb1.Finish();
        }
      }
      catch (Exception ex)
      {

      }
    }

    public static string CheckSchemaAndGetSchema(Element element, string guid, string name_para)
    {
      string value = "";
      try
      {
        Schema getSchema = Schema.Lookup(new Guid(guid));
        if (getSchema != null)
        {
          Entity ent = element.GetEntity(getSchema);
          if (ent.Schema != null)
          {
            value = ent.Get<string>(getSchema.GetField(name_para));
          }
        }
      }
      catch (Exception ex)
      {

      }
      return value;
    }

    public static ElementId CheckSchemaAndGetSchema_MaterialType(Element element, string guid, string name_para)
    {
      ElementId value = null;
      try
      {
        Schema getSchema = Schema.Lookup(new Guid(guid));
        if (getSchema != null)
        {
          Entity ent = element.GetEntity(getSchema);
          if (ent.Schema != null)
          {
            value = ent.Get<ElementId>(getSchema.GetField(name_para));
          }
        }
      }
      catch (Exception ex)
      {

      }
      return value;
    }

    public static void SetSchema(Element element, string guid, string name_para, string value)
    {
      try
      {
        Guid schemaGuid = new Guid(guid);

        // 1?? L?y schema n?u dã t?n t?i
        Schema schema = Schema.Lookup(schemaGuid);

        Entity entity = element.GetEntity(schema);
        if (!entity.IsValid())
          entity = new Entity(schema);

        Field field = schema.GetField(name_para);
        entity.Set(field, value);
        element.SetEntity(entity);

      }
      catch (Exception ex)
      {

      }
    }

    public static void SetSchema_MaterialType(Element element, string guid, string name_para, ElementId value)
    {
      try
      {
        SchemaBuilder sb1 = new SchemaBuilder(new Guid(guid));
        sb1.SetReadAccessLevel(AccessLevel.Public);
        sb1.SetWriteAccessLevel(AccessLevel.Public);
        sb1.SetVendorId("NMKhanh");
        sb1.SetSchemaName(name_para);

        FieldBuilder fieldUser = sb1.AddSimpleField(name_para, typeof(ElementId));
        fieldUser.SetDocumentation("Set");
        Schema schema = sb1.Finish();
        Entity entity = new Entity(schema);

        Field field = schema.GetField(name_para);
        entity.Set(field, value);
        element.SetEntity(entity);
      }
      catch (Exception ex)
      {

      }
    }
  }
}

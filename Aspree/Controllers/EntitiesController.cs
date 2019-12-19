using Aspree.ExtensionClasses;
using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    [CustomAuthorizeAttribute(Roles = "System Admin")]
    public class EntitiesController : BaseController
    {
        private readonly WebApiHandler _webApi;
        public EntitiesController()
        {
            _webApi = new WebApiHandler();
        }

        // GET: all form entities
        public ActionResult Index()
        {
            return View();
        }

        // GET: add new
        public ActionResult Create()
        {
            dynamic model = new System.Dynamic.ExpandoObject();

            model.TenantId = this.LoggedInUser.TenantId;

            var entity = _webApi.Get("Entity/GetByTenantId/" + model.TenantId);
            var entityValues = new Core.ViewModels.EntityViewModel();
            if (entity.MessageType == "Success")
            {
                var entityData = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.EntityViewModel>(entity.Content);
                entityValues = entityData;
            }

            var entities = new List<Core.ViewModels.EntityTypeViewModel>();
            var roleResponse = _webApi.Get("EntityType");
            if (roleResponse.MessageType == "Success")
            {
                entities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.EntityTypeViewModel>>(roleResponse.Content);
            }

            var entitiesSubType = new List<Core.ViewModels.EntitySubTypeViewModel>();
            var entitiessubtypeResponse = _webApi.Get("EntitySubType");
            if (entitiessubtypeResponse.MessageType == "Success")
            {
                entitiesSubType = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.EntitySubTypeViewModel>>(entitiessubtypeResponse.Content);
            }
            var categories = new List<Core.ViewModels.VariableCategoryViewModel>();
            var categoriesResponse = _webApi.Get("VariableCategory");
            if (categoriesResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.VariableCategoryViewModel>>(categoriesResponse.Content);
            }
            model.entities = entities;
            model.categories = categories;
            model.entityData = entityValues;
            model.entitiesSubType = entitiesSubType;
            ViewBag.EntityVariableList = new Core.ViewModels.EntityViewModel();
            return View(model);
        }

        // GET: edit
        public ActionResult Edit(Guid guid)
        {
            dynamic model = new System.Dynamic.ExpandoObject();

            model.TenantId = this.LoggedInUser.TenantId;
            //entity form variable
            var entityformdatavariableViewModel = new Core.ViewModels.EntityFormDataVariableViewModel();
            var entityformvariable = _webApi.Get("EntityFormDataVariable/" + guid);
            if (entityformvariable.MessageType == "Success")
            {
                entityformdatavariableViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.EntityFormDataVariableViewModel>(entityformvariable.Content);
            }
            ViewBag.entityformdatavariableViewModel = entityformdatavariableViewModel;
            Core.ViewModels.EntityFormDataVariableJson json = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.EntityFormDataVariableJson>(entityformdatavariableViewModel.Json);
            ViewBag.FormVariableDataJson = json;

            //GET / api / v1 / Entity / GetByTenantId{ guid}
            var entity = _webApi.Get("Entity/" + entityformdatavariableViewModel.EntityGuid.ToString());
            var entityValues = new Core.ViewModels.EntityViewModel();
            if (entity.MessageType == "Success")
            {
                var entityData = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.EntityViewModel>(entity.Content);
                entityValues = entityData;
            }
            var entities = new List<Core.ViewModels.EntityTypeViewModel>();
            var roleResponse = _webApi.Get("EntityType");

            if (roleResponse.MessageType == "Success")
            {
                entities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.EntityTypeViewModel>>(roleResponse.Content);
            }
            var entitiesSubType = new List<Core.ViewModels.EntitySubTypeViewModel>();
            var entitiessubtypeResponse = _webApi.Get("EntitySubType");

            if (entitiessubtypeResponse.MessageType == "Success")
            {
                entitiesSubType = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.EntitySubTypeViewModel>>(entitiessubtypeResponse.Content);
            }
            var categories = new List<Core.ViewModels.VariableCategoryViewModel>();
            var categoriesResponse = _webApi.Get("VariableCategory");

            if (categoriesResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.VariableCategoryViewModel>>(categoriesResponse.Content);
            }
            model.entities = entities;
            model.categories = categories;
            model.entityData = entityValues;
            model.entitiesSubType = entitiesSubType;
            return View(model);
        }

        // GET: configure
        public ActionResult Configuration()
        {
            dynamic model = new System.Dynamic.ExpandoObject();
           
            model.TenantId = this.LoggedInUser.TenantId;

            //GET / api / v1 / Entity / GetByTenantId{ guid}
            var entity = _webApi.Get("Entity/GetByTenantId" + model.TenantId);
            if (entity.MessageType == "Success")
            {
                var entityData = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.EntityViewModel>(entity.Content);
                model.entityData = entityData;
            }
            var entities = new List<Core.ViewModels.EntityTypeViewModel>();
            var roleResponse = _webApi.Get("EntityType");

            if (roleResponse.MessageType == "Success")
            {
                entities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.EntityTypeViewModel>>(roleResponse.Content);
            }

            var categories = new List<Core.ViewModels.VariableCategoryViewModel>();
            var categoriesResponse = _webApi.Get("VariableCategory");
            if (categoriesResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.VariableCategoryViewModel>>(categoriesResponse.Content);
            }

            model.entities = entities;
            model.categories = categories;

            return View(model);
        }

        public ActionResult AddEntityConfiguration()
        {
            dynamic model = new System.Dynamic.ExpandoObject();
            model.TenantId = this.LoggedInUser.TenantId; ;
            //GET / api / v1 / Entity / GetByTenantId{ guid}
            try
            {
                model.entityData = new Core.ViewModels.EntityViewModel();
                var entity = _webApi.Get("Entity/GetByTenantId" + model.TenantId);
                if (entity.MessageType == "Success")
                {
                    var entityData = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.EntityViewModel>(entity.Content);
                    model.entityData = entityData;
                }
            }
            catch (Exception EX)
            {

            }
            var entities = new List<Core.ViewModels.EntityTypeViewModel>();
            var roleResponse = _webApi.Get("EntityType");

            if (roleResponse.MessageType == "Success")
            {
                entities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.EntityTypeViewModel>>(roleResponse.Content);
            }

            var categories = new List<Core.ViewModels.VariableCategoryViewModel>();
            var categoriesResponse = _webApi.Get("VariableCategory");

            if (categoriesResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.VariableCategoryViewModel>>(categoriesResponse.Content);
            }
            model.entities = entities;
            model.categories = categories;
            return View(model);
        }

        public ActionResult EditEntityConfiguration(Guid guid)
        {
            dynamic model = new System.Dynamic.ExpandoObject();
            model.TenantId = this.LoggedInUser.TenantId;
            //GET / api / v1 / Entity / GetByTenantId{ guid}
            try
            {
                model.entityData = new Core.ViewModels.EntityViewModel();
                var entity = _webApi.Get("Entity/" + guid);
                if (entity.MessageType == "Success")
                {
                    var entityData = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.EntityViewModel>(entity.Content);
                    model.entityData = entityData;
                }
            }
            catch (Exception EX)
            {

            }
            var entities = new List<Core.ViewModels.EntityTypeViewModel>();
            var roleResponse = _webApi.Get("EntityType");

            if (roleResponse.MessageType == "Success")
            {
                entities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.EntityTypeViewModel>>(roleResponse.Content);
            }

            var categories = new List<Core.ViewModels.VariableCategoryViewModel>();
            var categoriesResponse = _webApi.Get("VariableCategory");

            if (categoriesResponse.MessageType == "Success")
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.VariableCategoryViewModel>>(categoriesResponse.Content);
            }
            var entitiesSubType = new List<Core.ViewModels.EntitySubTypeViewModel>();
            var entitiessubtypeResponse = _webApi.Get("EntitySubType");

            if (entitiessubtypeResponse.MessageType == "Success")
            {
                entitiesSubType = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.EntitySubTypeViewModel>>(entitiessubtypeResponse.Content);
            }
            ViewBag.entitiesSubType = entitiesSubType;

            model.entities = entities;
            model.categories = categories;
                return View(model);
        }

        public PartialViewResult VariablesList(Guid typeGuid, Guid? subTypeGuid = null)
        {
            Guid tenantGuid = this.LoggedInUser.TenantId;

            if (subTypeGuid != null)
            {
                var entityVariables = _webApi.Get("Entity/GetByEntityTypeAndSubTypeId/" + tenantGuid + "/" + typeGuid + "/" + subTypeGuid);
                if (entityVariables.MessageType == "Success")
                {
                    var entityData = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.EntityViewModel>(entityVariables.Content);
                    ViewBag.EntityVariableList = entityData;
                }
            }
            else
            {
                var entityVariables = _webApi.Get("Entity/GetByEntityTypeAndSubTypeId/" + tenantGuid + "/" + typeGuid + "/NULL");
                if (entityVariables.MessageType == "Success")
                {
                    var entityData = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.EntityViewModel>(entityVariables.Content);
                    ViewBag.EntityVariableList = entityData;
                }
            }
            return PartialView();
        }
    }
}
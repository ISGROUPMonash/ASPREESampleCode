using Aspree.Core.ViewModels.MongoViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface.MongoProvider
{
    public interface ISearchProvider
    {
        List<List<Core.ViewModels.FormDataEntryVariableViewModel>> SearchEntities(Core.ViewModels.SearchPageVariableViewModel model, string source = null);
        List<List<Core.ViewModels.FormDataEntryVariableViewModel>> TestEnvironment_SearchEntities(Core.ViewModels.SearchPageVariableViewModel model);

        Core.ViewModels.FormDataEntryViewModel Create(Core.ViewModels.FormDataEntryViewModel model);
        Core.ViewModels.FormDataEntryViewModel TestEnvironment_Create(Core.ViewModels.FormDataEntryViewModel model);

        Core.ViewModels.FormDataEntryViewModel UpdateSearchForm(string objId, Core.ViewModels.FormDataEntryViewModel model);
        Core.ViewModels.FormDataEntryViewModel TestEnvironment_UpdateSearchForm(string objId, Core.ViewModels.FormDataEntryViewModel model);

        Guid? GetCurrentAuthType(string userEntityObjId);
        Guid? TestEnvironment_GetCurrentAuthType(string userEntityObjId);

        void SaveChanges();

        string CheckEntityExistenceLocation(string entityId, Guid LoggedInUserId);

        FormDataEntryMongo Delete(string id,Guid loggedInUserId);
        FormDataEntryMongo TestEnvironment_Delete(string id, Guid loggedInUserId);


    }
}

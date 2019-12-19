using Aspree.Provider.Interface;
using Aspree.Provider.Provider;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace Aspree.WebApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            container.RegisterType<IRoleProvider, RoleProvider>();
            container.RegisterType<IVariableTypeProvider, VariableTypeProvider>();
            container.RegisterType<IVariableCategoryProvider, VariableCategoryProvider>();
            container.RegisterType<ICheckListProvider, CheckListProvider>();
            container.RegisterType<IFormStatusProvider, FormStatusProvider>();
            container.RegisterType<IValidationRuleProvider, ValidationRuleProvider>();
            container.RegisterType<IActivityCategoryProvider, ActivityCategoryProvider>();
            container.RegisterType<IActivityStatusProvider, ActivityStatusProvider>();
            container.RegisterType<IStatusProvider, StatusProvider>();
            container.RegisterType<IRegExRuleProvider, RegExRuleProvider>();
            container.RegisterType<IFormCategoryProvider, FormCategoryProvider>();
            container.RegisterType<IEntityTypeProvider, EntityTypeProvider>();
            container.RegisterType<IEntitySubTypeProvider, EntitySubTypeProvider>();
            container.RegisterType<ICountryProvider, CountryProvider>();
            container.RegisterType<IStateProvider, StateProvider>();
            container.RegisterType<ICityProvider, CityProvider>();
            container.RegisterType<ILookupDataProvider, LookupDataProvider>();
            container.RegisterType<ILookupDataGroupOptionProvider, LookupDataGroupOptionProvider>();
            container.RegisterType<ILookupDataOptionGroupProvider, LookupDataOptionGroupProvider>();
            container.RegisterType<ISiteProvider, SiteProvider>();
            container.RegisterType<IProjectStatusProvider, ProjectStatusProvider>();
            container.RegisterType<ISecurityQuestionProvider, SecurityQuestionProvider>();
            container.RegisterType<IUserLoginProvider, UserLoginProvider>();
            container.RegisterType<IPrivilegeProvider, PrivilegeProvider>();
            container.RegisterType<ITenantProvider, TenantProvider>();
            container.RegisterType<IDashboardProvider, DashboardProvider>();
            container.RegisterType<IForgotPasswordProvider, ForgotPasswordProvider>();
            container.RegisterType<IValidatorProvider, ValidatorProvider>();
            container.RegisterType<IEmailTemplateProvider, EmailTemplateProvider>();
            container.RegisterType<IPushEmailEventProvider, PushEmailEventProvider>();
            container.RegisterType<IProjectProvider, ProjectProvider>();
            container.RegisterType<IVariableProvider, VariableProvider>();
            container.RegisterType<IEntityProvider, EntityProvider>();
            container.RegisterType<IFormProvider, FormProvider>();
            container.RegisterType<IActivityProvider, ActivityProvider>();
            container.RegisterType<IEntityFormDataVariableProvider, EntityFormDataVariableProvider>();
            container.RegisterType<IFormDataEntryProvider, FormDataEntryProvider>();
            container.RegisterType<IPostCodeProvider, PostCodeProvider>();
            container.RegisterType<ISchedulingProvider, SchedulingProvider>();
            container.RegisterType<IAuthenticationTypeProvider, AuthenticationTypeProvider>();

            container.RegisterType<Aspree.Provider.Interface.MongoProvider.IProjectDeployProvider, Aspree.Provider.Provider.MongoProvider.ProjectDeployProvider>();
            container.RegisterType<Aspree.Provider.Interface.MongoProvider.ISearchProvider, Aspree.Provider.Provider.MongoProvider.SearchProvider>();
            container.RegisterType<Aspree.Provider.Interface.MongoProvider.ISummaryProvider, Aspree.Provider.Provider.MongoProvider.SummaryProvider>();

            container.RegisterType<IReportProvider, ReportProvider>();
            container.RegisterType<IActionListProvider, ActionListProvider>();
        }

    }
}
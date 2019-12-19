using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using Aspree.WebApi.Utilities;
using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UserController : BaseController
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly IForgotPasswordProvider _forgotPasswordProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLoginProvider"></param>
        /// <param name="forgotPasswordProvider"></param>
        public UserController(IUserLoginProvider userLoginProvider, IForgotPasswordProvider forgotPasswordProvider)
        {
            this._userLoginProvider = userLoginProvider;
            this._forgotPasswordProvider = forgotPasswordProvider;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <remarks>
        /// Get all users<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all users.
        /// - This api returns list of UserLoginViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<UserLoginViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllUserLoginViewModelExamples))]
        public IEnumerable<UserLoginViewModel> GetAll()
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            return _userLoginProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get user by guid
        /// </summary>
        /// <remarks>
        /// Get user by guid<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api is used to get user by its guid.
        /// - This api returns UserLoginViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an user that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]   
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserLoginViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(UserLoginViewModelExamples))]
        public UserLoginViewModel Get(Guid guid)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            var user = _userLoginProvider.GetByGuid(guid);
            if (user == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("User was not found."),
                    ReasonPhrase = "User was not found."
                });
            }

            return user;
        }

        /// <summary>
        /// Get user profile by guid
        /// </summary>
        /// <remarks>
        /// Get user profile by guid<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api is used to get user profile by its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an user that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]  
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserLoginViewModel))]
        [Route("api/v1/user/profile/{guid}")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public HttpResponseMessage Profile(Guid guid)
        {
            #region check login
            //if (Request.Headers.Authorization == null)
            //{
            //    var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("Unauthorized access. Please login.")
            //    };
            //    throw new HttpResponseException(msg);
            //}
            #endregion
            var imgPath = System.IO.Path.Combine(ConfigSettings.ProfileImageBasePath + "/", guid.ToString() + ".jpg");
            if (System.IO.File.Exists(imgPath))
            {
                var stream = System.IO.File.ReadAllBytes(imgPath);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream)
                };
                result.Content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return result;
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Add new user
        /// </summary>
        /// <remarks>
        /// Add new user<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to create new user.
        /// - This api takes NewUserViewModel of an user as input request.
        /// - This api returns UserLoginViewModel of new created user in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newUser">NewUserViewModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "record already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserLoginViewModel))]
        [SwaggerRequestExample(typeof(NewUserViewModel), typeof(NewUserViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(UserLoginViewModelExamples))]
        public HttpResponseMessage Post([FromBody]NewUserViewModel newUser)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var added = _userLoginProvider.Create(new UserLoginViewModel()
            {
                Address = newUser.Address,
                AuthTypeId = (int)Core.Enum.AuthenticationTypes.Local_Password,
                CreatedBy = this.LoggedInUserId,
                CreatedDate = DateTime.UtcNow,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Mobile = newUser.Mobile,
                RoleId = newUser.RoleId,
                TenantId = newUser.TenantId,
                Guid = Guid.NewGuid(),
                TempGuid = Guid.NewGuid(),
                UserTypeId = (int)Core.Enum.UsersLoginType.System,
                UserName = newUser.UserName,
                IsUserApprovedBySystemAdmin = newUser.IsUserApprovedBySystemAdmin,
            });

            _userLoginProvider.SaveChanges();

            SaveProfileImage(added.Guid, newUser.Profile);

            var emailService = new Services.EmailService();

            var isSent = emailService.SendWelcomeEmail(added.Email, added.FirstName + " " + added.LastName, Utilities.ConfigSettings.WebUrl + "account/resetpassword/" + added.TempGuid.ToString());


            return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        private void SaveProfileImage(Guid userGuid, string imageData)
        {
            try
            {
                var imgPath = System.IO.Path.Combine(ConfigSettings.ProfileImageBasePath + "/", userGuid.ToString() + ".jpg");
                if (string.IsNullOrEmpty(imageData))
                {
                    //System.IO.File.Delete(imgPath);
                    return;
                }
                using (var fileStream = System.IO.File.OpenWrite(imgPath))
                {
                    byte[] file = Convert.FromBase64String(imageData.Split(',')[1]);
                    fileStream.Write(file, 0, file.Length);
                }

            }
            catch (Exception exp)
            {

                throw;
            }
        }

        /// <summary>
        /// Edit existing user
        /// </summary>
        /// <remarks>
        /// Edit existing user<br></br> 
        /// <strong>Purpose.</strong>        
        /// - This api is used to update an existing user.
        /// - This api takes EditUserViewModel and guid of user as input request.
        /// - This api returns UserLoginViewModel of updated user in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editUser">Edit EditUserViewModel Model</param>
        /// <param name="guid">guid of user that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "records already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserLoginViewModel))]
        [SwaggerRequestExample(typeof(EditUserViewModel), typeof(EditUserViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(UserLoginViewModelExamples))]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditUserViewModel editUser)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var updated = _userLoginProvider.UpdateMyProfile(new UserLoginViewModel()
            {
                Guid = guid,
                Address = editUser.Address,
                AuthTypeId = (int)Core.Enum.AuthenticationTypes.Local_Password,
                ModifiedBy = this.LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Email = editUser.Email,
                FirstName = editUser.FirstName,
                LastName = editUser.LastName,
                Mobile = editUser.Mobile,
                RoleId = editUser.RoleId,
                TenantId = editUser.TenantId,
                UserName = editUser.UserName,
                Status = editUser.Status,
                IsUserApprovedBySystemAdmin = editUser.IsUserApprovedBySystemAdmin,
            });

            _userLoginProvider.SaveChanges();

            SaveProfileImage(updated.Guid, editUser.Profile);

            if (updated != null)
                return Request.CreateResponse(HttpStatusCode.OK, updated);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "User was not found.");
        }

        /// <summary>
        /// Delete existing user
        /// </summary>
        /// <remarks>
        /// Delete existing user<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to delete an existing user.
        /// - This api takes guid of an user that needs to be deleted as input request.
        /// - This api returns UserLoginViewModel of deleted user in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of user that needs to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserLoginViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(UserLoginViewModelExamples))]
        public HttpResponseMessage Delete(Guid guid)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            var response = _userLoginProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _userLoginProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "User was not found.");
        }

        /// <summary>
        /// Get user by temp guid
        /// </summary>
        /// <remarks>
        /// Get user by temp guid<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to get user by tempguid.
        /// - This api returns UserLoginViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Temp Guid of an user that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]   
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserLoginViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(UserLoginViewModelExamples))]
        [Route("api/v1/user/GetUserByTempGuid/{guid}")]
        [AllowAnonymous]
        [HttpGet]
        public UserLoginViewModel GetUserByTempGuid(Guid? guid)
        {
            if (guid == null)
            {
                GuidExceptionHandler();
            }
            var newguid = guid.Value;

            var user = _userLoginProvider.GetByTempGuid(newguid);
            if (user == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("User was not found."),
                    ReasonPhrase = "User was not found."
                });
            }

            return user;
        }

        /// <summary>
        /// Get user by main id
        /// </summary>
        /// <remarks>
        /// Get user by main id<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api is to get user details by id.
        /// - This api returns UserLoginViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="id">id of an user that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]   
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserLoginViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(UserLoginViewModelExamples))]
        [Route("api/v1/user/GetUserById/{id}")]
        [HttpGet]
        public UserLoginViewModel GetUserById(int id)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            var user = _userLoginProvider.GetById(id);
            if (user == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("User was not found."),
                    ReasonPhrase = "User was not found."
                });
            }

            return user;
        }

        /// <summary>
        /// Get project users
        /// </summary>
        /// <remarks>
        /// Get project users<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api is used to get all users of a project.
        /// - This api takes project id as input request.
        /// - This api returns list of UserLoginViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Project id to get project users</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<UserLoginViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllUserLoginViewModelExamples))]
        [Route("api/v1/user/GetProjectAllUsers/{projectId}")]
        [HttpGet]
        public IEnumerable<UserLoginViewModel> GetProjectAllUsers(Guid projectId)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            return _userLoginProvider.GetProjectAllUsers(projectId);
        }

        /// <summary>
        /// Get all users for system admin tools
        /// </summary>
        /// <remarks>
        /// Get all users for system admin tools<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all users for system admin tools
        /// - This api returns list of UserLoginViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">project id</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<SystemAdminToolsUserViewModel>))]
        [Route("api/v1/user/GetAllSystemAdminToolsUser/{projectId}")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<SystemAdminToolsUserViewModel> GetAllSystemAdminToolsUser(Guid projectId)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            return _userLoginProvider.GetAllSystemAdminToolsUser(this.LoggedInUserTenantId, projectId);
        }



        /// <summary>
        /// Get all users for system admin tools
        /// </summary>
        /// <remarks>
        /// Get all users for system admin tools<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all users for system admin tools
        /// - This api returns list of UserLoginViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="username">project id</param>
        /// <param name="isTestSite">is test site</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<SystemAdminToolsUserViewModel>))]
        [Route("api/v1/user/GetUserStatus/{username}/{isTestSite}/")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public int GetUserStatus(string username, bool isTestSite = false)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            //if (this.LoggedInUserId == Guid.Empty)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("Unauthorized access. Please login."),
            //    });
            //}
            #endregion
            return _userLoginProvider.GetUserStatus(username, (int)Core.Enum.AuthenticationTypes.Local_Password, isTestSite);
        }

    }
}
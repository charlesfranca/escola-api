using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API.Controllers
{
    [Route("api/[controller]")]
    public class VideoController : Controller
    {
        Services.Interfaces.IVideoService _videoService;

        public VideoController(Services.Interfaces.IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.VideoViewModel>> Get(List<Guid> categoriesId)
        {
            try
            {
                var data = _videoService.GetVideos(new Services.Message.GetVideosRequest(){ categoriesId = categoriesId }).videos;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.VideoViewModel>>.CreateResponse(true, "", data);
            }
            catch (Infrastructure.BusinessRuleException bex)
            {
                return Infrastructure.ApiResponse<IList<Services.ViewModel.VideoViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }
            catch (Exception ex)
            {
                return Infrastructure.ApiResponse<IList<Services.ViewModel.VideoViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("UpdateMedia")]
        public async Task<bool> UpdateMedia()
        {
            var sambatechresponse = await ApiRequestHelper.Get<List<EscolaDeVoce.Services.ViewModel.VideoSambatech>>("http://api.sambavideos.sambatech.com/v1/medias?access_token=181e463a-034b-4ea5-878b-cea906a5f2e2&pid=6023");
            var videosResponse = _videoService.GetVideos(new Services.Message.GetVideosRequest());

            foreach (var m in sambatechresponse)
            {
                var video = videosResponse.videos.Where(v => v.sambatech_id == m.id).FirstOrDefault();

                if (video != null)
                {
                    video.thumbs = new List<EscolaDeVoce.Services.ViewModel.ThumbViewModel>();
                    video.files = new List<EscolaDeVoce.Services.ViewModel.FileViewModel>();
                    video.sambatech_id = m.id;
                    video.thumb = m.thumbs[m.thumbs.Count - 1].url;
                    video.duration = m.files[0].fileInfo.duration;

                    var updatevideoreques = new Services.Message.UpdateVideoRequest();
                    updatevideoreques.video = video;
                    var updateresponse = _videoService.UpdateVideo(updatevideoreques);
                }

            }

            return true;
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel> Get(string id)
        {
            try
            {
                Guid videoid = Guid.Empty;
                if (!Guid.TryParse(id, out videoid))
                    return Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel>.CreateResponse(false, "Video não encontrado", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetVideoRequest req = new Services.Message.GetVideoRequest();
                req.videoid = videoid;
                var video = _videoService.GetVideo(req).video;
                return Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel>.CreateResponse(true, "", video);
            }
            catch (Infrastructure.BusinessRuleException bex)
            {
                return Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }
            catch (Exception ex)
            {
                return Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("getStatus/{userid}/{videoid}")]
        public Infrastructure.ApiResponse<int> GetuserStatus(string userid, string videoid)
        {
            try
            {
                Guid id = Guid.Empty;
                if (!Guid.TryParse(userid, out id))
                    return Infrastructure.ApiResponse<int>.CreateResponse(false, "Video não encontrado", 0, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetVideoStatusRequest req = new Services.Message.GetVideoStatusRequest();
                req.model = new Services.ViewModel.GetVideoStatusViewModel();
                req.model.userId = id;
                req.model.videoId = Guid.Parse(videoid);
                var videos = _videoService.GetVideoStatus(req).status;
                return Infrastructure.ApiResponse<int>.CreateResponse(true, "", videos);
            }
            catch (Infrastructure.BusinessRuleException bex)
            {
                return Infrastructure.ApiResponse<int>.CreateResponse(false, bex.Message, 0, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }
            catch (Exception ex)
            {
                return Infrastructure.ApiResponse<int>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", 0, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("userfavorites/{userid}")]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.VideoViewModel>> Getuserfavorites(string userid)
        {
            try
            {
                Guid id = Guid.Empty;
                if (!Guid.TryParse(userid, out id))
                    return Infrastructure.ApiResponse<IList<Services.ViewModel.VideoViewModel>>.CreateResponse(false, "Video não encontrado", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetFavoriteVideosRequest req = new Services.Message.GetFavoriteVideosRequest();
                req.userid = id;
                var videos = _videoService.GetFavoriteVideos(req).videos;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.VideoViewModel>>.CreateResponse(true, "", videos);
            }
            catch (Infrastructure.BusinessRuleException bex)
            {
                return Infrastructure.ApiResponse<IList<Services.ViewModel.VideoViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }
            catch (Exception ex)
            {
                return Infrastructure.ApiResponse<IList<Services.ViewModel.VideoViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("updateVideoStatus")]
        public Infrastructure.ApiResponse<bool> UpdateVideoStatus([FromBody]Services.ViewModel.UpdateVideoStatusViewModel model)
        {
            var response = new Infrastructure.ApiResponse<bool>();
            try
            {
                var req = new Services.Message.UpdateVideoProgressRequest();
                req.status = model;

                _videoService.UpdateVideoProgress(req);

                response.status = true;
                response.data = true;
                response.code = System.Net.HttpStatusCode.Created;
            }
            catch (Infrastructure.BusinessRuleException bex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.BadRequest;
                response.brokenRules = bex.BrokenRules;
                response.error_message = bex.Message;
            }
            catch (Exception ex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.InternalServerError;
                response.error_message = "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.";
            }
            return response;
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel> Post([FromBody]Services.ViewModel.VideoViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel>();
            try
            {
                var req = new Services.Message.AddVideoRequest();
                req.video = model;

                _videoService.AddVideo(req);

                response.status = true;
                response.data = model;
                response.code = System.Net.HttpStatusCode.Created;
            }
            catch (Infrastructure.BusinessRuleException bex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.BadRequest;
                response.brokenRules = bex.BrokenRules;
                response.error_message = bex.Message;
            }
            catch (Exception ex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.InternalServerError;
                response.error_message = "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.";
            }
            return response;
        }

        [HttpPost("addVideoToFavorites")]
        public Infrastructure.ApiResponse<bool> AddToFavorites([FromBody]Services.ViewModel.AddVideoToFavoriteViewModel model)
        {
            var response = new Infrastructure.ApiResponse<bool>();
            try
            {
                var req = new Services.Message.AddVideoToFavoritesRequest();
                req.model = model;

                _videoService.AddVideoToFavoritesCourse(req);

                response.status = true;
                response.data = true;
                response.code = System.Net.HttpStatusCode.Created;
            }
            catch (Infrastructure.BusinessRuleException bex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.BadRequest;
                response.brokenRules = bex.BrokenRules;
                response.error_message = bex.Message;
            }
            catch (Exception ex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.InternalServerError;
                response.error_message = "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.";
            }
            return response;
        }

        [HttpPut("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel> Put(string id, [FromBody]Services.ViewModel.VideoViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel>();
            try
            {
                Guid videoid = Guid.Empty;
                if (!Guid.TryParse(id, out videoid))
                    return Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel>.CreateResponse(false, "Video não encontrado", null, System.Net.HttpStatusCode.NotFound);

                model.Id = videoid;
                _videoService.UpdateVideo(new Services.Message.UpdateVideoRequest() { video = model });

                response.status = true;
                response.data = model;
                response.code = System.Net.HttpStatusCode.Created;
            }
            catch (Infrastructure.BusinessRuleException bex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.BadRequest;
                response.brokenRules = bex.BrokenRules;
                response.error_message = bex.Message;
            }
            catch (Exception ex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.InternalServerError;
                response.error_message = "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.";
            }

            return response;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel>();
            try
            {
                Guid videoid = Guid.Empty;
                if (!Guid.TryParse(id, out videoid))
                    return Infrastructure.ApiResponse<Services.ViewModel.VideoViewModel>.CreateResponse(false, "Video não encontrado", null, System.Net.HttpStatusCode.NotFound);

                _videoService.RemoveVideo(new Services.Message.RemoveVideoRequest() { videoid = videoid });

                response.status = true;
                response.data = null;
                response.code = System.Net.HttpStatusCode.Created;
            }
            catch (Infrastructure.BusinessRuleException bex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.BadRequest;
                response.brokenRules = bex.BrokenRules;
                response.error_message = bex.Message;
            }
            catch (Exception ex)
            {
                response.status = true;
                response.code = System.Net.HttpStatusCode.InternalServerError;
                response.error_message = "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.";
            }

            return response;
        }
    }
}

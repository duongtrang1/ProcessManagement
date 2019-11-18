﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProcessManagement.Models;
namespace ProcessManagement.Services
{
	public class ParticipateService
	{
		///=============================================================================================
		PMSEntities db = new PMSEntities();
		///=============================================================================================

		/// <summary>
		/// Tạo mới một particition
		/// </summary>
		/// <param name="idUser">Id User</param>
		/// <param name="idGroup">Id Group</param>
		/// <param name="part">Model Participate</param>
		/// <param name="isOwner">Có phải owner hông??</param>
		public void createParticipate(string idUser, int idGroup, Participate part, bool isOwner)
		{

			part.IdGroup = idGroup;
			part.IdUser = idUser;
			if (isOwner)
			{
				part.IsOwner = true;
				part.IsAdmin = true;
				part.IsManager = true;
			}
			else
			{
				part.IsOwner = false;
				part.IsAdmin = false;
				part.IsManager = false;
			}
			part.Created_At = DateTime.Now;
			part.Updated_At = DateTime.Now;
			db.Participates.Add(part);
			db.SaveChanges();
		}
		/// <summary>
		/// Thay đổi role của một user
		/// </summary>
		/// <param name="model">Participate model</param>
		public void editRoleUser(Participate model)
		{
            Participate user = findMemberInGroup(model.Id);
			user.IsAdmin = model.IsAdmin;
			user.IsManager = model.IsManager;
			user.Updated_At = DateTime.Now;
			db.Entry(user).State = System.Data.Entity.EntityState.Modified;
			db.SaveChanges();
		}
		/// <summary>
		/// Xóa user ra khỏi group
		/// </summary>
		/// <param name="participate">Oarticipate Model</param>
		public void removeUserInGroup(Participate participate)
		{
			db.Participates.Remove(participate);
			db.SaveChanges();
		}
		public void removeUsersInGroup(List<Participate> listUser)
		{
			db.Participates.RemoveRange(listUser);
			db.SaveChanges();
		}
		/// <summary>
		/// Tìm một user thuộc group đó
		/// </summary>
		/// <param name="idParticipant">Id Participant</param>
		/// <returns>Thông tin user đó</returns>
		public Participate findMemberInGroup(int idParticipant)
		{
			Participate user = db.Participates.SingleOrDefault(x => x.Id == idParticipant);
			return user;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public Participate findMemberInGroup(string idUser, int idGroup)
        {
            Participate user = db.Participates.SingleOrDefault(x => x.IdUser == idUser && x.IdGroup == idGroup);
            return user;
        }
        public int countMemberInGroup(int IdGroup)
        {
            int count = db.Participates.Where(x => x.IdGroup == IdGroup).Count();
            return count;
        }
        /// <summary>
        /// Tìm tất cả các member thuộc group đó
        /// </summary>
        /// <param name="IdGroup">id group</param>
        /// <param name="quantity">số lượng muốn lấy,mặc định lấy tất cả</param>
        /// <returns>return danh sách member của group đó</returns>
		public List<Participate> findMembersInGroup(int IdGroup, int quantity = -1)
		{
            IQueryable<Participate> ListParticipant = db.Participates.Where(x => x.IdGroup == IdGroup);
            if(quantity != -1)
            {
                ListParticipant = ListParticipant.Take(quantity);
            }
			return ListParticipant.ToList();
		}
        /// <summary>
        /// Lấy tất cả các member trong group trừ owner
        /// </summary>
        /// <param name="IdGroup">id group</param>
        /// <param name="quantity">số lượng muốn lấy,mặc định lấy tất cả</param>
        /// <returns>return danh sách member của group đó</returns>
        public List<Participate> findMembersNotOwnerInGroup(int IdGroup, int quantity = -1)
        {
            Group group = db.Groups.Find(IdGroup);
            IQueryable<Participate> ListParticipant = db.Participates.Where(x => x.IdGroup == IdGroup && x.IdUser != group.IdOwner);
            if (quantity != -1)
            {
                ListParticipant = ListParticipant.Take(quantity);
            }
            return ListParticipant.ToList();
        }
        /// <summary>
        /// Tìm tất cả member không thuộc group đó
        /// </summary>
        /// <param name="memberInGroup">List các Members thuộc group đó</param>
        /// <returns>Return danh sách các Members không thuộc group đó</returns>
        public List<AspNetUser> findMembersNotInGroup(List<Participate> memberInGroup,string key = null)
		{
            List<string> userInGroup = new List<string>();
            foreach (Participate item in memberInGroup)
            {
                userInGroup.Add(item.IdUser);
            }
            //string temp = String.Join(", ", userInGroup); 
            List<AspNetUser> memberNotInGroup = db.AspNetUsers.Where(x => !userInGroup.Contains(x.Id)).OrderByDescending(x => x.Id).ToList();
            return memberNotInGroup;
        }
        public List<AspNetUser> searchMembersNotInGroup(List<Participate> memberInGroup, string key = null,int quantity = 5)
        {
            List<string> userInGroup = new List<string>();
            foreach (Participate item in memberInGroup)
            {
                userInGroup.Add(item.IdUser);
            }
            //string temp = String.Join(", ", userInGroup); 
            List<AspNetUser> memberNotInGroup;
            if (key != null)
                memberNotInGroup = db.AspNetUsers.Where(x => !userInGroup.Contains(x.Id) && x.NickName.ToLower().Contains(key)).OrderByDescending(x => x.Id).Take(quantity).ToList();
            else
                memberNotInGroup = db.AspNetUsers.Where(x => !userInGroup.Contains(x.Id)).OrderByDescending(x => x.Id).Take(quantity).ToList();
            return memberNotInGroup;
        }
        public bool checkMemberInGroup(string userid, int groupid)
        {
            Participate user = db.Participates.FirstOrDefault(x => x.IdUser == userid && x.IdGroup == groupid);
            return user != null ? true : false;
        }
        /// <summary>
        /// Lấy role của member trong một group
        /// </summary>
        /// <param name="idUser">Id Member</param>
        /// <param name="idGroup">Id Group</param>
        /// <returns>Return một object Participate của member thuộc group đó</returns>
        public Participate getRoleOfMember(string idUser, int idGroup)
		{
            Participate role = db.Participates.Where(x => x.IdUser == idUser && x.IdGroup == idGroup).FirstOrDefault();
			return role;
		}
		/// <summary>
		/// Add nhiều member vào group
		/// </summary>
		/// <param name="group">Model Group</param>
		/// <param name="listUser">Danh sách email của member</param>
		public void addMembers(Group group, List<string> listUser)
		{
			foreach (string user in listUser)
			{
				Participate role = new Participate();
				role.IdUser = db.AspNetUsers.SingleOrDefault(x => x.Email == user).Id;
				role.IdGroup = group.Id;
				role.Created_At = DateTime.Now;
				role.Updated_At = DateTime.Now;

				db.Participates.Add(role);
			}
			db.SaveChanges();
		}
        public void addMember(string idUser, int idGroup)
        {
            Participate role = new Participate();
            role.IdUser = idUser;
            role.IdGroup = idGroup;
            role.Created_At = DateTime.Now;
            role.Updated_At = DateTime.Now;
            db.Participates.Add(role);
            db.SaveChanges();
        }
        public Participate checkMemberExist(string idUser, int idGroup)
        {
            Participate user = db.Participates.FirstOrDefault(x => x.IdUser == idUser && x.IdGroup == idGroup);
            return user;
        }
	}
}
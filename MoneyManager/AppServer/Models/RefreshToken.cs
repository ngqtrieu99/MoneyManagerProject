﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppServer.Models
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public string? Token { get; set; }

        public string? JwtId { get; set; }

        public bool IsUsed { get; set; }

        public bool IsReVoke { get; set; }

        public DateTime? IssuedAt { get; set; }

        public DateTime? ExpiredAt { get; set; }
    }
}
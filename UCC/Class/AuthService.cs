using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UCC.Model;

namespace UCC.Class
{
    public class AuthService
    {
        /// <summary>
        /// Выполняет авторизацию пользователя по email и паролю
        /// </summary>
        /// <returns>
        /// "Patient" — пациент,
        /// "Doctor" — врач (не админ),
        /// "Admin" — администратор,
        /// null — ошибка авторизации
        /// </returns>
        public async Task<string> LoginAsync(string email, string password)
        {
            return await Task.Run(() =>
            {
                using (var db = new ECCEntities1()) // ← ЗАМЕНИТЕ НА ВАШЕ ИМЯ DbContext
                {
                    // ПОИСК В ПАЦИЕНТАХ
                    var patient = db.Patients.FirstOrDefault(p =>
                        p.Email == email &&
                        p.PasswordHash == password); // ← сравниваем напрямую

                    if (patient != null)
                    {
                        return "Patient";
                    }

                    // ПОИСК В СОТРУДНИКАХ
                    var staff = db.Staff.FirstOrDefault(s =>
                        s.Email == email &&
                        s.PasswordHash == password); // ← сравниваем напрямую

                    if (staff != null)
                    {
                        // ОПРЕДЕЛЕНИЕ РОЛИ СОТРУДНИКА
                        var medicalRole = db.MedicalRoles.FirstOrDefault(r => r.RoleId == staff.RoleId);
                        if (medicalRole != null)
                        {
                            if (medicalRole.RoleName == "Администратор")
                                return "Admin";
                            else
                                return "Doctor";
                        }
                    }

                    return null; // Пользователь не найден
                }
            });
        }

        /// <summary>
        /// Регистрация нового пациента
        /// </summary>
        public async Task<bool> RegisterPatientAsync(
            string fullName,
            DateTime dateOfBirth,
            string phone,
            string address,
            string email,
            string password)
        {
            return await Task.Run(() =>
            {
                using (var db = new ECCEntities1())
                {
                    // Проверка уникальности email
                    if (db.Patients.Any(p => p.Email == email))
                        return false;

                    // Создаём запись в Patients (пароль в открытом виде)
                    var patient = new Patients
                    {
                        FullName = fullName,
                        DateOfBirth = dateOfBirth,
                        Phone = phone,
                        Address = address,
                        Email = email,
                        PasswordHash = password, // ← пароль в открытом виде
                        CreatedAt = DateTime.Now
                    };
                    db.Patients.Add(patient);
                    db.SaveChanges();

                    // Создаём амбулаторную карту
                    var medicalCard = new MedicalCards
                    {
                        PatientId = patient.PatientId,
                        OpenedAt = DateTime.Now
                    };
                    db.MedicalCards.Add(medicalCard);

                    db.SaveChanges();
                    return true;
                }
            });
        }
    }
}
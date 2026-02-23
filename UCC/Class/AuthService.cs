using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UCC.Model;

// Установите пакет: Install-Package BCrypt.Net-Next
using BC = BCrypt.Net.BCrypt;

namespace UCC.Class
{
    public class AuthService
    {
        /// <summary>
        /// Выполняет авторизацию пользователя по email и паролю
        /// </summary>
        public async Task<string> LoginAsync(string email, string password)
        {
            return await Task.Run(() =>
            {
                using (var db = new ECCEntities1())
                {
                    // ПОИСК В ПАЦИЕНТАХ (только BCrypt)
                    var patient = db.Patients.FirstOrDefault(p => p.Email == email);
                    if (patient != null && BC.Verify(password, patient.PasswordHash))
                    {
                        return "Patient";
                    }

                    // ПОИСК В СОТРУДНИКАХ (поддержка открытого текста + миграция на BCrypt)
                    var staff = db.Staff.FirstOrDefault(s => s.Email == email);
                    if (staff != null)
                    {
                        bool isValid = false;

                        // Попытка 1: BCrypt (новые пароли)
                        try
                        {
                            if (BC.Verify(password, staff.PasswordHash))
                            {
                                isValid = true;
                            }
                        }
                        catch (Exception)
                        {
                            // Игнорируем ошибки BCrypt (например, "Invalid salt version")
                        }

                        // Попытка 2: Открытый текст (старые пароли)
                        if (!isValid && staff.PasswordHash == password)
                        {
                            isValid = true;
                            // Мигрируем на BCrypt
                            staff.PasswordHash = BC.HashPassword(password);
                            db.SaveChanges();
                        }

                        if (isValid)
                        {
                            // ОПРЕДЕЛЕНИЕ РОЛИ
                            var role = db.MedicalRoles.FirstOrDefault(r => r.RoleId == staff.RoleId);
                            if (role != null && role.RoleName == "Администратор")
                            {
                                return "Admin";
                            }
                            else
                            {
                                return "Doctor";
                            }
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
            string password,
            byte[] photo = null)
        {
            return await Task.Run(() =>
            {
                using (var db = new ECCEntities1())
                {
                    if (db.Patients.Any(p => p.Email == email))
                        return false;

                    var patient = new Patients
                    {
                        FullName = fullName,
                        DateOfBirth = dateOfBirth,
                        Phone = phone,
                        Address = address,
                        Email = email,
                        PasswordHash = BC.HashPassword(password), // BCrypt
                        CreatedAt = DateTime.Now,
                        Image = photo
                    };
                    db.Patients.Add(patient);
                    db.SaveChanges();

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
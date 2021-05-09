using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Transfer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RekognitionPPE
{
    public partial class FormMain : Form
    {
        AWSCredentials credentials;
        private static readonly RegionEndpoint region = RegionEndpoint.USEast1;

        private const string bucketName = "bucket-ppe";

        private readonly DrawBox draw = new();

        public FormMain()
        {
            InitializeComponent();
        }

        public void GetCredentials()
        {
            var chain = new CredentialProfileStoreChain();
            if (!chain.TryGetAWSCredentials("AWS Educate", out credentials))
            {
                MessageBox.Show("Erro ao obter credenciais");
            }
        }

        private async Task<bool> UploadToS3(string file)
        {
            GetCredentials();
            try
            {
                var client = new AmazonS3Client(credentials, region);
                var fileTransfer = new TransferUtility(client);
                await fileTransfer.UploadAsync(file, bucketName);
                return true;
            }
            catch (AmazonS3Exception e)
            {
                MessageBox.Show($"Erro: { e.Message}");
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro: {e.Message}");
                return false;
            }
        }

        private async Task DetectedPPE(string imageName)
        {
            var client = new AmazonRekognitionClient(credentials, region);
            var detectedPpeRequest = new DetectProtectiveEquipmentRequest()
            {
                Image = new Amazon.Rekognition.Model.Image()
                {
                    S3Object = new S3Object()
                    {
                        Bucket = bucketName,
                        Name = Path.GetFileName(imageName)
                    }
                },
                SummarizationAttributes = new ProtectiveEquipmentSummarizationAttributes()
                {
                    MinConfidence = 80,
                    RequiredEquipmentTypes = new List<String>() { "FACE_COVER" }
                }
            };
            try
            {
                var detectedPeeResponse = await client.DetectProtectiveEquipmentAsync(detectedPpeRequest);
                bool hasAll = detectedPpeRequest.SummarizationAttributes.RequiredEquipmentTypes.Contains("FACE_COVER");
                RtbReturn.Clear();
                foreach (var person in detectedPeeResponse.Persons)
                {
                    RtbReturn.AppendText(JsonConvert.SerializeObject(person, Formatting.Indented));
                    draw.DrawPerson(person, PictureBox);
                    foreach (var bodyParts in person.BodyParts)
                    {
                        foreach(var equipment in bodyParts.EquipmentDetections)
                        {
                            draw.DrawEquipment(equipment, PictureBox);

                        }
                    }
                }
            }
            catch(AmazonS3Exception e)
            {
                MessageBox.Show($"Erro: { e.Message}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro: {e.Message}");
            }
        }

        private async void BtSearch_Click(object sender, EventArgs e)
        {
            OpenFileDialog.ShowDialog();
            PictureBox.Load(OpenFileDialog.FileName);
            await UploadToS3(OpenFileDialog.FileName);
            await DetectedPPE(OpenFileDialog.FileName);
        }
    }
}
